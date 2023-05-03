using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Xaml;
using System.Xml;
using Microsoft.Win32;
using Newtonsoft.Json;
using NoteWPF.Model;
using NoteWPF.ViewModel;
using Formatting = Newtonsoft.Json.Formatting;
using XamlWriter = System.Windows.Markup.XamlWriter;

namespace NoteWPF.Behavior;

public class RTBehaviour:Behavior<FrameworkElement>
{
    public Window WindowNote = null;
    public RichTextBox RichTextControl = null;
    public ComboBox FontBackColor = null;
    public ComboBox FontHeight = null;
    public Button btnSave = null; 
    public Button btnOpen = null;
    
    protected override void OnAttached()
    {
        try
        {
            base.OnAttached();
            if (WindowNote == null)
                WindowNote = (Window)AssociatedObject;
            if (WindowNote == null) return;
            RichTextControl = (RichTextBox)WindowNote.FindName("RichTextControl");
            RichTextControl.SelectionChanged += RichTextControl_SelectionChanged;
            RichTextControl.LostFocus += RichTextControl_LostFocus;
            FontBackColor = (ComboBox)WindowNote.FindName("ColorBox");
            FontHeight = (ComboBox)WindowNote.FindName("Fontheight");
            btnSave = (Button)WindowNote.FindName("btnSave");
            btnOpen = (Button)WindowNote.FindName("btnOpen");
            btnSave.Click += SaveRtf;
            btnOpen.Click += OpenRtf;
            RichTextControl.Loaded += RichTextControl_Loaded;
            FontBackColor.DropDownClosed += FontFamilyType_DropDownClosed;
            FontHeight.DropDownClosed += FontHeight_DropDownClosed;

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OpenRtf(object sender, RoutedEventArgs e)
    {
        try
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Rtf файл (*.rtf)|*.rtf";
            if (openFileDialog.ShowDialog() == true)
            {
                using (FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open))
                {
                    Data.Data.nvm.Name = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    TextRange range = new TextRange(RichTextControl.Document.ContentStart,
                        RichTextControl.Document.ContentEnd);
                    range.Load(fileStream, DataFormats.Rtf);
                    RichTextControl.Document.DataContext = range;
                }
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show("Ошибка при загрузке файла: " + exception.Message);
        }
    }


    // List<Tuple<TextPointer, TextPointer>> GetFormattedTextRanges()
    // {
    //     List<Tuple<TextPointer, TextPointer>> ranges = new List<Tuple<TextPointer, TextPointer>>();
    //
    //     TextPointer start = rtb.Document.ContentStart;
    //     TextPointer end = rtb.Document.ContentEnd;
    //
    //     while (start.CompareTo(end) < 0)
    //     {
    //         if (start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart)
    //         {
    //             string elementName = start.GetAdjacentElement(LogicalDirection.Forward).GetType().Name;
    //             switch (elementName)
    //             {
    //                 case "Run":
    //                     TextPointer runEnd = start.GetNextContextPosition(LogicalDirection.Forward);
    //                     ranges.Add(new Tuple<TextPointer, TextPointer>(start, runEnd));
    //                     start = runEnd;
    //                     break;
    //
    //                 case "Paragraph":
    //                     TextPointer paraEnd = start.GetNextContextPosition(LogicalDirection.Forward);
    //                     ranges.Add(new Tuple<TextPointer, TextPointer>(start, paraEnd));
    //                     start = paraEnd;
    //                     break;
    //
    //                 default:
    //                     start = start.GetNextContextPosition(LogicalDirection.Forward);
    //                     break;
    //             }
    //         }
    //         else
    //         {
    //             start = start.GetNextContextPosition(LogicalDirection.Forward);
    //         }
    //     }
    //
    //     return ranges;
    // }
    
    
    private void SaveRtf(object sender, RoutedEventArgs e)
    {
        var selectionRange = new TextRange(RichTextControl.Document.ContentStart, RichTextControl.Document.ContentEnd);
        using (FileStream fileStream = new FileStream(Data.Data.Note.Text + ".rtf", FileMode.Create))
        {
            TextRange range = new TextRange(RichTextControl.Document.ContentStart, RichTextControl.Document.ContentEnd);
            range.Save(fileStream, DataFormats.Rtf);
        }
        //
        // var settings = new JsonSerializerSettings
        // {
        //     PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        //     Formatting = Formatting.Indented
        // };
        //
        // string json = JsonConvert.SerializeObject(selectionRange, settings);
        
        using (FileStream fileStream = new FileStream(Data.Data.Note.Name+".rtf", FileMode.Create))
        {
            TextRange range = new TextRange(RichTextControl.Document.ContentStart, RichTextControl.Document.ContentEnd);
            range.Save(fileStream, DataFormats.Rtf);
        }
        
        // var ranges = GetFormattedTextRanges();
        //
        //
        // Data.Data.Note.Styles = ranges
        //     .Select(range => new NoteStyle
        //     {
        //         Property = range.Item1.BackgroundProperty.ToString(),
        //         Value = range.GetPropertyValue(TextElement.FontSizeProperty)
        //     })
        //     .ToList();
        //
        // Data.Data.Note
        //
        // return JsonConvert.SerializeObject(Data.Data.Note);
        
    }
    
    private void RichTextControl_SelectionChanged(object sender, RoutedEventArgs e)
    {
        try
        {
            var selectionRange = new TextRange(RichTextControl.Selection.Start, RichTextControl.Selection.End);

            FontBackColor.SelectedValue = selectionRange.GetPropertyValue(FlowDocument.FontFamilyProperty).ToString();
            FontHeight.SelectedValue = selectionRange.GetPropertyValue(FlowDocument.FontSizeProperty).ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void FontHeight_DropDownClosed(object sender, EventArgs e)
    {
        var fontHeightValue = (string)FontHeight.SelectedItem;
        if (fontHeightValue == null) return;
        RichTextControl.Selection.ApplyPropertyValue(Control.FontSizeProperty, fontHeightValue);
        RichTextControl.Focus();
    }

    private void FontFamilyType_DropDownClosed(object sender, EventArgs e)
    {
        var fontNameValue = (string)FontBackColor.SelectedItem;
        if (fontNameValue == null) return;
        
        SolidColorBrush br = Brushes.Beige;

        switch (fontNameValue)
        {
            case "White":
                br = Brushes.White;
                break;
            case "LightGray":
                br = Brushes.LightGray;
                break;
            case "DarkGray":
                br = Brushes.DarkGray;
                break;
            case "Black":
                br = Brushes.Black;
                break;
            case "Red":
                br = Brushes.Red;
                break;
            case "Green":
                br = Brushes.Green;
                break;
            case "Blue":
                br = Brushes.Blue;
                break;
        }
        TextRange selectionRange = new TextRange(RichTextControl.Selection.Start, RichTextControl.Selection.End);
        selectionRange.ApplyPropertyValue(TextElement.BackgroundProperty, br);
        // RichTextControl.Selection.ApplyPropertyValue(Control.BackgroundProperty, br);
        RichTextControl.Focus();
    }
    
    private void RichTextControl_LostFocus(object sender, RoutedEventArgs e)
    {
        var range = new TextRange(RichTextControl.Document.ContentStart, RichTextControl.Document.ContentEnd);
        WindowNote.Uid = Regex.Replace(range.Text, @"\t|\n|\r", "");
        WindowNote.Tag = range.Text;
    }

    private void RichTextControl_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var range = new TextRange(RichTextControl.Selection.Start, RichTextControl.Selection.End);
            FontHeight.SelectedValue = range.GetPropertyValue(FlowDocument.FontSizeProperty).ToString();
            
            RichTextControl.Focus();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
}
