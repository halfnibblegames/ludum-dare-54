using Godot;

public class BottomHud : Control
{
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

        label.BbcodeText = $"{name}\n[color=silver]{desc}[/color][color=teal]{useDesc}[/color]";
        frame.RectMinSize = new Vector2(0, label.RectSize.y + 3);
        Visible = true;
    }
}
