using LOGIYGames.Shared.Enums;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class InputCommandBuffer
    {
        private readonly List<IComboInputCommand>
            bufferedCommands = new();

        // =====================================================
        // ADD
        // =====================================================

        public void AddCommand(
            IComboInputCommand command)
        {
            //CleanupExpiredCommands();

            bufferedCommands.Add(command);
        }

        // =====================================================
        // MATCH
        // =====================================================

        public int GetMatchLength(
            IReadOnlyList<AttackInputType> sequence)
        {
            //CleanupExpiredCommands();

            if (sequence == null
                || sequence.Count == 0)
            {
                return 0;
            }

            if (bufferedCommands.Count
                < sequence.Count)
            {
                return 0;
            }

            int startIndex =
                bufferedCommands.Count
                - sequence.Count;

            int matched = 0;

            for (int i = 0;
                 i < sequence.Count;
                 i++)
            {
                if (bufferedCommands[startIndex + i]
                        .InputType
                    != sequence[i])
                {
                    return 0;
                }

                matched++;
            }

            return matched;
        }

        // =====================================================
        // CLEAR
        // =====================================================

        public void Clear()
        {
            bufferedCommands.Clear();
        }

        // =====================================================
        // DEBUG
        // =====================================================

        public string GetDebugBuffer()
        {

            if (bufferedCommands.Count == 0)
                return "[EMPTY]";

            StringBuilder builder =
                new();

            for (int i = 0;
                 i < bufferedCommands.Count;
                 i++)
            {
                builder.Append(
                    bufferedCommands[i]
                        .InputType);

                if (i < bufferedCommands.Count - 1)
                {
                    builder.Append(
                        " -> ");
                }
            }

            return builder.ToString();
        }
    }
}
