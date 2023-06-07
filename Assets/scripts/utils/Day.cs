
using System;
using System.ComponentModel;
using System.Reflection;

public enum Day
{
    [Description("Monday")]
    Monday,
    [Description("Tuesday")]
    Tuesday,
    [Description("Wednesday")]
    Wednesday,
    [Description("Thursday")]
    Thursday,
    [Description("Friday")]
    Friday,
    [Description("Saturday")]
    Saturday,
    [Description("Sunday")]
    Sunday
}

public class DescriptionAttribute : Attribute
{
    public string Description;

    public DescriptionAttribute(string description)
    {
        Description = description;
    }
}

public static class  EnumHelper
{
    public static string GetDescription(this Enum enumValue)
    {
        Type enumType = enumValue.GetType();
        FieldInfo enumField = enumType.GetField(enumValue.ToString());
        DescriptionAttribute[] enumInfo = 
            (DescriptionAttribute[])enumField
            .GetCustomAttributes(typeof(DescriptionAttribute), false);
        string description = enumInfo.Length > 0 ? 
            enumInfo[0].Description : 
            enumValue.ToString();
        return description;
    }
}