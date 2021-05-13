/*
Redeclaration of methods to hide them from IntelliSense.
EditorBrowsableAttribute does not suppress members from a class in the same assembly.
Editorbrowsable does not suppress members if you're referencing another project in the solution (?)
In Resharper Options, go to:
Environment | IntelliSense | Completion Appearance and check “Filter members by [EditorBrowsable] attribute”.
*/

using System;
using System.ComponentModel;

namespace InterReact
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IEditorBrowsableNever
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetType();

        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object obj);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class EditorBrowsableNever
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Type GetType() => base.GetType();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => base.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => base.ToString();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => ReferenceEquals(this, obj);
    }

}