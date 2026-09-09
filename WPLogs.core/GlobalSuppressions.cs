// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0057:Use range operator", Justification = "call me old fashioned, but it is easier to read the code when there is item.Body.Substring(0, 100)", Scope = "module")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "It makes it easier to read the code this way", Scope = "module")]
