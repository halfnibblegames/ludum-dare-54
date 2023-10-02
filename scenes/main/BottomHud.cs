using Godot;

public sealed class BottomHud : Control
{
    public override void _Process(float delta)
    {
        var label = GetNode<RichTextLabel>("Label");
        var frame = GetNode<NinePatchRect>("Frame");
        // Something overwrites this so we're stubborn and just reset it every frame
        frame.RectMinSize = new Vector2(0, label.RectSize.y + 3);
    }

    public void ClearItem()
    {
        var label = GetNode<RichTextLabel>("Label");
        var frame = GetNode<NinePatchRect>("Frame");

        label.BbcodeText = "";
        frame.RectMinSize = Vector2.Zero;
        Visible = false;
    }

    public void SetItem(ItemLibrary.ItemType item)
    {
        var label = GetNode<RichTextLabel>("Label");
        var frame = GetNode<NinePatchRect>("Frame");

        var name = item.ToString();
        var desc = item.Description();
        var useDesc = item.CombatUse() is { } use ? $"\n{use.Description}" : "";
        var scoreDesc = item.Score() switch
        {
            1 => "\n1 point",
            0 => "",
            _ => $"\n{item.Score()} points",
        };

        label.BbcodeText = $"{name}\n[color=silver]{desc}[/color][color=teal]{useDesc}[/color][color=yellow]{scoreDesc}[/color]";
        frame.RectMinSize = new Vector2(0, label.RectSize.y + 3);
        Visible = true;
    }
}
