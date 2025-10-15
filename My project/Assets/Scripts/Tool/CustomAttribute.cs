using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class EditableStaticAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class ReadonlyStaticAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method ,AllowMultiple =false,Inherited = true)]
public class VoidStaticMethodAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class TrackStaticAttribute : Attribute { }