using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inf1nity.Commands
{
    public class HelpModule : ModuleBase
    {
        private readonly CommandService _commands;
        private readonly IServiceProvider _serviceProvider;

        public HelpModule(IServiceProvider serviceProvider, CommandService commands)
        {
            _commands = commands;
            _serviceProvider = serviceProvider;
        }

        [Command("help")]
        [Summary("Lists this bot's commands.")]
        public async Task Help(string path = "")
        {
            await Context.Message.DeleteAsync();
            var output = new EmbedBuilder();
            if (path == "")
            {
                output.Title = "Inf1nity by `@InfinityGhost#7843`";

                foreach (var mod in _commands.Modules.Where(m => m.Parent == null && m.Name != "HelpModule"))
                    AddHelp(mod, ref output);

                output.Footer = new EmbedFooterBuilder
                {
                    Text = "Use 'help <module>' to get help with a module."
                };
            }
            else
            {
                var mod = _commands.Modules.FirstOrDefault(m => m.Name.Replace("Module", "").ToLower() == path.ToLower());
                if (mod == null) { await ReplyAsync("No module could be found with that name."); return; }

                output.Title = mod.Name;
                output.Description = $"{mod.Summary}{Environment.NewLine}" +
                (!string.IsNullOrEmpty(mod.Remarks) ? $"({mod.Remarks}){Environment.NewLine}" : "") +
                (mod.Aliases.Any() ? $"Prefix(es): {string.Join(",", mod.Aliases)}{Environment.NewLine}" : "") +
                (mod.Submodules.Any() ? $"Submodules: {mod.Submodules.Select(m => m.Aliases)}{Environment.NewLine}" : "") + " ";
                AddCommands(mod, ref output);
            }

            await ReplyAsync("", embed: output.Build());
        }

        public void AddHelp(ModuleInfo module, ref EmbedBuilder builder)
        {
            foreach (var sub in module.Submodules) AddHelp(sub, ref builder);
            builder.AddField(f =>
            {
                f.Name = $"**{module.Name}**";
                f.Value = $"Submodules: {string.Join(", ", module.Submodules.Select(m => m.Name))}" +
                $"{Environment.NewLine}" +
                $"Commands: {string.Join(", ", module.Commands.Select(x => $"`{x.Name}`"))}";
            });
        }

        public void AddCommands(ModuleInfo module, ref EmbedBuilder builder)
        {
            foreach (var command in module.Commands)
            {
                command.CheckPreconditionsAsync(Context, _serviceProvider).GetAwaiter().GetResult();
                AddCommand(command, ref builder);
            }

        }

        public void AddCommand(CommandInfo command, ref EmbedBuilder builder)
        {
            builder.AddField(f =>
            {
                f.Name = $"**{command.Name}**";
                f.Value = $"{command.Summary}{Environment.NewLine}" +
                (!string.IsNullOrEmpty(command.Remarks) ? $"({command.Remarks}){Environment.NewLine}" : "") +
                (command.Aliases.Any() ? $"**Aliases:** {string.Join(", ", command.Aliases.Select(x => $"`{x}`"))}{Environment.NewLine}" : "") +
                $"**Usage:** `{GetPrefix(command)} {GetAliases(command)}`";
            });
        }

        public string GetAliases(CommandInfo command)
        {
            StringBuilder output = new StringBuilder();
            if (!command.Parameters.Any()) return output.ToString();
            foreach (var param in command.Parameters)
            {
                if (param.IsOptional)
                    output.Append($"[{param.Name} = {param.DefaultValue}] ");
                else if (param.IsMultiple)
                    output.Append($"|{param.Name}| ");
                else if (param.IsRemainder)
                    output.Append($"...{param.Name} ");
                else
                    output.Append($"<{param.Name}> ");
            }
            return output.ToString();
        }
        public string GetPrefix(CommandInfo command)
        {
            var output = GetPrefix(command.Module);
            output += $"{command.Aliases.FirstOrDefault()} ";
            return output;
        }
        public string GetPrefix(ModuleInfo module)
        {
            string output = "";
            if (module.Parent != null) output = $"{GetPrefix(module.Parent)}{output}";
            if (module.Aliases.Any())
                output += string.Concat(module.Aliases.FirstOrDefault(), " ");
            return output;
        }
    }
}
