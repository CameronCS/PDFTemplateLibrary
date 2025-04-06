namespace PDFTemplateLibrary.Iterators;

public class IterationMember {
    public int Start { get; set; }
    public int End { get; set; }
    public int Current { get; set; }
    public string As { get; set; }
    
    public string EndEvaluator { get; set; }
}