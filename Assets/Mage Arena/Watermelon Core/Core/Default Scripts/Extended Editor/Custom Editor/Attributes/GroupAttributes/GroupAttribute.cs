using System;

namespace Watermelon
{
    public abstract class GroupAttribute : ExtendedEditorAttribute
    {
        private string name;

        public GroupAttribute(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }
}