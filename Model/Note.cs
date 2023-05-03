using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace NoteWPF.Model;
[Serializable]
public class Note
{
    public string Name { get; set; }
    public string Text { get; set; }
    public List<NoteStyle> Styles { get; set; }
    
    public Note()
    {
        Name = "Note";
        Text = "";
        Styles = new List<NoteStyle>();
    }
}

public class NoteStyle
{
    public string Property { get; set; }
    public object Value { get; set; }
}

