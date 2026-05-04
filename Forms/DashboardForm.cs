using Pawductivity.Managers;
using Pawductivity.Models;
using Pawductivity.Persistence;
using Pawductivity.Animations;
using System.Linq;

namespace Pawductivity.Forms;

public class DashboardForm : Form
{
    private readonly GameManager _gm;

    // Pet panel widgets
    private Label _lblPetEmoji = null!;
    private Label _lblPetName = null!;
    private Label _lblGreeting = null!;
    private Label _lblLevel = null!;
    private ProgressBar _pbHealth = null!;
    private ProgressBar _pbMood = null!;
    private ProgressBar _pbXp = null!;
    private Label _lblCoins = null!;
    private AnimationPanel _animPanel = null!;

    // Task panel widgets
    private ListView _lvTasks = null!;
    private Button _btnAddTask = null!;
    private Button _btnComplete = null!;
    private Button _btnDelete = null!;
    private Button _btnEdit = null!;

    // Nav buttons
    private Button _btnShop = null!;
    private Button _btnStats = null!;

    // Stats labels
    private Label _lblToday = null!;
    private Label _lblStreak = null!;
    private Label _lblPending = null!;

    private System.Windows.Forms.Timer _decayTimer = null!;

    // ── Layout constants ─────────────────────────────────────────────
    private new int Margin = 16;
    private const int InnerPad = 14;
    private const int TopBarH = 52;
    private const int PetPanelW = 288;
    private const int ButtonH = 34;
    private const int NavButtonW = 126;
    private const int ToolbarButtonW = 126;
    private const int StatBarH = 14;
    private const int StatBarLblGap = 4;

    public DashboardForm(GameManager gm)
    {
        _gm = gm;
        InitializeComponent();
        RefreshAll();
        StartDecayTimer();

        // ── Save on exit ─────────────────────────────────────────────
        FormClosed += (s, e) =>
        {
            _animPanel.Dispose();
            SaveManager.Save(_gm);
            Application.Exit();
        };
    }

    private void InitializeComponent()
    {
        Text = "Pawductivity 🐾 — Dashboard";
        MinimumSize = new Size(970, 670);
        Size = new Size(930, 650);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = PawTheme.Background;
        Font = PawTheme.FontBody;

        // ── TOP BAR ──────────────────────────────────────────────────
        var topBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = TopBarH,
            BackColor = PawTheme.Primary,
        };

        var lblApp = new Label
        {
            Text = "🐾 Pawductivity",
            Font = new Font("Segoe UI", 16f, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(Margin, (TopBarH - 28) / 2),
        };

        var lblUser = new Label
        {
            Text = $"Hi, {_gm.UserName}! 💕",
            Font = new Font("Segoe UI", 9f),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = true,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
        };
        topBar.Controls.AddRange([lblApp, lblUser]);
        lblUser.Location = new Point(topBar.Width - 200, (TopBarH - 18) / 2);

        // ── LEFT: PET PANEL ──────────────────────────────────────────
        int panelTop = TopBarH + Margin;
        int panelBottom = ClientSize.Height - Margin;
        int petPanelH = panelBottom - panelTop;

        var petPanel = new Panel
        {
            Location = new Point(Margin, panelTop),
            Size = new Size(PetPanelW, petPanelH),
            BackColor = PawTheme.Surface,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
        };
        petPanel.Paint += (s, e) => PaintBorder(e, petPanel);

        _lblPetEmoji = new Label          // keep the field — RefreshAll still sets its Text
        {
            Font = new Font("Segoe UI Emoji", 52f),
            AutoSize = false,
            Size = new Size(PetPanelW - InnerPad * 2, 110),
            Location = new Point(InnerPad, InnerPad + 6),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent,
            Visible = false,              // ← HIDE the static emoji; the animator draws instead
        };

        // ── Animated pet canvas (replaces the static emoji) ──────────────
        var petType = _gm.Pet is CatPet ? PetType.Cat : PetType.Dog;
        _animPanel = new AnimationPanel(petType)
        {
            Location = new Point(InnerPad, InnerPad + 6),
            Size = new Size(PetPanelW - InnerPad * 2, 110),
        };

        _animPanel.Animator.SetStage(_gm.Pet.Stage);

        _lblPetName = new Label
        {
            Font = new Font("Segoe UI", 13f, FontStyle.Bold),
            ForeColor = PawTheme.Primary,
            AutoSize = false,
            Size = new Size(PetPanelW - InnerPad * 2, 26),
            Location = new Point(InnerPad, _lblPetEmoji.Bottom + 8),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent,
        };

        _lblGreeting = new Label
        {
            Font = PawTheme.FontSmall,
            ForeColor = PawTheme.TextMuted,
            AutoSize = false,
            Size = new Size(PetPanelW - InnerPad * 2, 36),
            Location = new Point(InnerPad, _lblPetName.Bottom + 4),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent,
        };

        _lblLevel = new Label
        {
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            ForeColor = PawTheme.TextDark,
            AutoSize = true,
            Location = new Point(InnerPad, _lblGreeting.Bottom + 10),
            BackColor = Color.Transparent,
        };

        int barSlotH = 10 + StatBarLblGap + StatBarH + 8;
        int firstBarY = _lblLevel.Location.Y + 20 + 10;

        var (lblH, _pbHealthOut) = MakeStatBar("❤️ Health", firstBarY, PawTheme.HealthBar);
        var (lblM, _pbMoodOut) = MakeStatBar("😸 Mood", firstBarY + barSlotH, PawTheme.MoodBar);
        var (lblX, _pbXpOut) = MakeStatBar("⭐ XP", firstBarY + barSlotH * 2, PawTheme.XpBar);

        _pbHealth = _pbHealthOut;
        _pbMood = _pbMoodOut;
        _pbXp = _pbXpOut;

        _lblCoins = new Label
        {
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            ForeColor = PawTheme.Primary,
            AutoSize = true,
            Location = new Point(InnerPad, _pbXp.Bottom + 12),
            BackColor = Color.Transparent,
        };

        var statPanel = new Panel
        {
            Location = new Point(InnerPad, _lblCoins.Location.Y + 24 + 8),
            Size = new Size(PetPanelW - InnerPad * 2, 90),
            BackColor = PawTheme.Background,
        };

        _lblToday = MakeStatLabel("Tasks today: 0", new Point(6, 6));
        _lblStreak = MakeStatLabel("🔥 Streak: 0 days", new Point(6, 34));
        _lblPending = MakeStatLabel("📋 Pending: 0", new Point(6, 62));
        statPanel.Controls.AddRange([_lblToday, _lblStreak, _lblPending]);

        int navBtnY = statPanel.Bottom + 12;
        _btnShop = new Button { Text = "🛍 Shop", Location = new Point(InnerPad, navBtnY), Width = NavButtonW, Height = ButtonH };
        _btnStats = new Button { Text = "📊 Stats", Location = new Point(InnerPad + NavButtonW + 8, navBtnY), Width = NavButtonW, Height = ButtonH };

        PawTheme.StyleButton(_btnShop, outlined: true);
        PawTheme.StyleButton(_btnStats, outlined: true);
        _btnShop.Click += (s, e) => new ShopForm(_gm, RefreshAll, _animPanel).ShowDialog(this);
        _btnStats.Click += (s, e) => new StatsForm(_gm).ShowDialog(this);

        petPanel.Controls.AddRange([
            _lblPetEmoji, _lblPetName, _lblGreeting, _lblLevel,
            lblH, _pbHealth, lblM, _pbMood, lblX, _pbXp,
            _lblCoins, statPanel, _btnShop, _btnStats,
        ]);

        petPanel.Controls.AddRange([
            _animPanel,          // ← ADD THIS (add before _lblPetEmoji)
            _lblPetEmoji, _lblPetName, _lblGreeting, _lblLevel,
            lblH, _pbHealth, lblM, _pbMood, lblX, _pbXp,
            _lblCoins, statPanel, _btnShop, _btnStats,
        ]);

        // ── RIGHT: TASK PANEL ────────────────────────────────────────
        int taskPanelX = Margin + PetPanelW + Margin;
        int taskPanelW = Width - taskPanelX - Margin - 16;

        var taskPanel = new Panel
        {
            Location = new Point(taskPanelX, panelTop),
            Size = new Size(taskPanelW, petPanelH),
            BackColor = PawTheme.Surface,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right,
        };
        taskPanel.Paint += (s, e) => PaintBorder(e, taskPanel);

        var lblTaskTitle = new Label
        {
            Text = "📋 My Tasks",
            Font = PawTheme.FontHeading,
            ForeColor = PawTheme.Primary,
            AutoSize = true,
            Location = new Point(InnerPad, InnerPad),
            BackColor = Color.Transparent,
        };

        int listTop = lblTaskTitle.Bottom + 10;
        int listH = petPanelH - listTop - ButtonH - InnerPad * 2 - 4;

        _lvTasks = new ListView
        {
            Location = new Point(InnerPad, listTop),
            Size = new Size(taskPanelW - InnerPad * 2, listH),
            View = View.Details,
            FullRowSelect = true,
            GridLines = false,
            HotTracking = true,
            BackColor = PawTheme.Background,
            ForeColor = PawTheme.TextDark,
            Font = PawTheme.FontBody,
            BorderStyle = BorderStyle.None,
            Anchor = AnchorStyles.Top | AnchorStyles.Left |
                            AnchorStyles.Bottom | AnchorStyles.Right,
        };

        _lvTasks.Columns.Add("", 30);
        _lvTasks.Columns.Add("Task", 220);
        _lvTasks.Columns.Add("Priority", 96);
        _lvTasks.Columns.Add("Due Date", 96);
        _lvTasks.Columns.Add("Status", 96);

        _lvTasks.OwnerDraw = true;
        _lvTasks.ColumnWidthChanging += (s, e) =>
        {
            e.Cancel = true;
            e.NewWidth = _lvTasks.Columns[e.ColumnIndex].Width;
        };
        _lvTasks.Resize += (s, e) =>
        {
            int used = _lvTasks.Columns[0].Width + _lvTasks.Columns[2].Width
                     + _lvTasks.Columns[3].Width + _lvTasks.Columns[4].Width;
            _lvTasks.Columns[1].Width = _lvTasks.ClientSize.Width - used;
        };
        _lvTasks.DrawColumnHeader += (s, e) =>
        {
            using var bg = new SolidBrush(PawTheme.Secondary);
            e.Graphics.FillRectangle(bg, e.Bounds);
            using var pen = new Pen(Color.FromArgb(40, PawTheme.TextDark));
            e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1,
                                     e.Bounds.Right, e.Bounds.Bottom - 1);
            TextRenderer.DrawText(e.Graphics, e.Header.Text, _lvTasks.Font, e.Bounds,
                                  PawTheme.TextDark,
                                  TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        };
        _lvTasks.DrawItem += LvTasks_DrawItem;
        _lvTasks.DrawSubItem += LvTasks_DrawSubItem;

        int toolY = petPanelH - InnerPad - ButtonH;
        int toolGap = 10;

        _btnAddTask = new Button { Text = "+ Add Task", Width = ToolbarButtonW, Height = ButtonH, Location = new Point(InnerPad, toolY) };
        _btnComplete = new Button { Text = "✔ Complete", Width = ToolbarButtonW, Height = ButtonH, Location = new Point(InnerPad + (ToolbarButtonW + toolGap), toolY) };
        _btnEdit = new Button { Text = "✏️ Edit", Width = ToolbarButtonW, Height = ButtonH, Location = new Point(InnerPad + (ToolbarButtonW + toolGap) * 2, toolY) };
        _btnDelete = new Button { Text = "🗑 Delete", Width = ToolbarButtonW, Height = ButtonH, Location = new Point(InnerPad + (ToolbarButtonW + toolGap) * 3, toolY) };

        foreach (var btn in new[] { _btnAddTask, _btnComplete, _btnEdit, _btnDelete })
            btn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

        PawTheme.StyleButton(_btnAddTask);
        PawTheme.StyleButton(_btnComplete);
        PawTheme.StyleButton(_btnEdit, outlined: true);
        PawTheme.StyleButton(_btnDelete, outlined: true);

        _btnAddTask.Click += BtnAdd_Click;
        _btnComplete.Click += BtnComplete_Click;
        _btnEdit.Click += BtnEdit_Click;
        _btnDelete.Click += BtnDelete_Click;

        taskPanel.Controls.AddRange([
            lblTaskTitle, _lvTasks,
            _btnAddTask, _btnComplete, _btnEdit, _btnDelete,
        ]);

        Controls.AddRange([topBar, petPanel, taskPanel]);
    }

    // ── CUSTOM LISTVIEW DRAW ─────────────────────────────────────────
    private void LvTasks_DrawItem(object? sender, DrawListViewItemEventArgs e)
    {
        e.DrawDefault = false;
        if (e.Item?.Tag is not TaskItem task) return;

        bool sel = (e.State & ListViewItemStates.Selected) != 0;
        Color bg = sel ? PawTheme.Secondary : _lvTasks.BackColor;

        using var brush = new SolidBrush(bg);
        e.Graphics.FillRectangle(brush, e.Bounds);
    }

    private void LvTasks_DrawSubItem(object? sender, DrawListViewSubItemEventArgs e)
    {
        if (e.Item?.Tag is not TaskItem task) return;

        bool sel = (e.ItemState & ListViewItemStates.Selected) != 0;
        Color bg = sel ? PawTheme.Secondary : _lvTasks.BackColor;

        using var brush = new SolidBrush(bg);
        e.Graphics.FillRectangle(brush, e.Bounds);

        var flags = e.ColumnIndex == 0
            ? TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix
            : TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;

        TextRenderer.DrawText(e.Graphics, e.SubItem?.Text, _lvTasks.Font, e.Bounds, PawTheme.TextDark, flags);
    }


    // ── BUTTON HANDLERS ──────────────────────────────────────────────
    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var dlg = new TaskEditForm();
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            _gm.AddTask(dlg.Result!);
            RefreshAll();
        }
    }

    private void BtnComplete_Click(object? sender, EventArgs e)
    {
        if (_lvTasks.SelectedItems.Count == 0) return;
        var selected = _lvTasks.SelectedItems[0];
        if (selected?.Tag is not TaskItem task) { ShowInfo("No task selected."); return; }
        if (task.IsCompleted) { ShowInfo("This task is already done! 🎉"); return; }

        int levelBefore = _gm.Pet.Level;           // ← capture level BEFORE

        _gm.CompleteTask(task.Id);

        int xpGained = task.Priority switch
        {
            TaskPriority.High => _gm.Pet is CatPet ? 30 : 25,
            TaskPriority.Medium => _gm.Pet is CatPet ? 20 : 15,
            _ => _gm.Pet is CatPet ? 10 : 8,
        };
        int coins = task.Priority switch
        {
            TaskPriority.High => 15,
            TaskPriority.Medium => 10,
            _ => 5,
        };

        // ── Trigger animations ──────────────────────────────────────────
        _animPanel.Animator.TriggerTaskComplete(xpGained, coins);
        FloatyLabel.Show(this, _lblCoins, $"+{coins} 🪙", FloatyLabel.CoinColor);

        if (_gm.Pet.Level > levelBefore)
            _animPanel.Animator.TriggerLevelUp(_gm.Pet.Level);   // level-up burst

        ShowInfo($"{_gm.Pet.GetGreeting()}\n\n+XP gained! 🌟 Coins earned: {coins} 🪙");
        RefreshAll();
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        if (_lvTasks.SelectedItems.Count == 0) return;
        var task = (TaskItem)_lvTasks.SelectedItems[0].Tag!;
        using var dlg = new TaskEditForm(task);
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            _gm.EditTask(task.Id, dlg.Result!.Title, dlg.Result.Description,
                         dlg.Result.Priority, dlg.Result.DueDate);
            RefreshAll();
        }
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (_lvTasks.SelectedItems.Count == 0) return;
        var task = (TaskItem)_lvTasks.SelectedItems[0].Tag!;
        if (MessageBox.Show($"Delete \"{task.Title}\"?", "Confirm",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            _gm.DeleteTask(task.Id);
            RefreshAll();
        }
    }

    // ── REFRESH ──────────────────────────────────────────────────────
    public void RefreshAll()
    {
        var pet = _gm.Pet;
        _lblPetEmoji.Text = $"{pet.StageEmoji}\n{pet.MoodEmoji}";
        _lblPetName.Text = pet.Name;
        _lblGreeting.Text = pet.GetGreeting();
        _lblLevel.Text = $"Lv.{pet.Level} • Stage: {pet.Stage}";
        _lblCoins.Text = $"🪙 Coins: {pet.Coins}";
        _pbHealth.Value = pet.Health;
        _pbMood.Value = pet.Mood;
        _pbXp.Value = Math.Min(100, (int)((double)pet.XP / pet.XpForNextLevel * 100));

        _lblToday.Text = $"✅ Completed today: {_gm.CompletedToday}";
        _lblStreak.Text = $"🔥 Streak: {_gm.CurrentStreak} day(s)";
        _lblPending.Text = $"📋 Pending: {_gm.PendingCount}";

        _lvTasks.Items.Clear();
        foreach (var t in _gm.Tasks.OrderBy(t => t.IsCompleted).ThenBy(t => t.DueDate))
        {
            var item = new ListViewItem(t.IsCompleted ? "✅" : t.IsOverdue ? "⚠️" : "⬜");
            item.SubItems.Add(t.Title);
            item.SubItems.Add($"{t.PriorityEmoji} {t.Priority}");
            item.SubItems.Add(t.DueDate.ToString("MMM dd"));
            item.SubItems.Add(t.IsCompleted ? "Done 🎉" : t.IsOverdue ? "Overdue!" : "Pending");
            item.Tag = t;
            _lvTasks.Items.Add(item);
        }
        _animPanel.Animator.SetStage(_gm.Pet.Stage);
    }


    // ── DECAY TIMER ──────────────────────────────────────────────────
    private void StartDecayTimer()
    {
        _decayTimer = new System.Windows.Forms.Timer { Interval = 60_000 };
        _decayTimer.Tick += (s, e) =>
        {
            bool hadOverdue = _gm.Tasks.Any(t => t.IsOverdue);
            _gm.ApplyOverduePenalties();
            RefreshAll();
            if (hadOverdue)
                _animPanel.Animator.TriggerOverdue();   // ← show health/mood loss animation
        };
        _decayTimer.Start();
    }

    // ── HELPERS ──────────────────────────────────────────────────────
    private static (Label lbl, ProgressBar pb) MakeStatBar(string label, int y, Color color)
    {
        var lbl = new Label
        {
            Text = label,
            Font = PawTheme.FontSmall,
            ForeColor = PawTheme.TextMuted,
            AutoSize = true,
            Location = new Point(InnerPad, y),
            BackColor = Color.Transparent,
        };
        var pb = new ProgressBar
        {
            Location = new Point(InnerPad, y + 15 + StatBarLblGap),
            Width = PetPanelW - InnerPad * 2,
            Height = StatBarH,
            Style = ProgressBarStyle.Continuous,
            Maximum = 100,
            Minimum = 0,
        };
        return (lbl, pb);
    }

    private static Label MakeStatLabel(string text, Point loc) => new()
    {
        Text = text,
        Font = PawTheme.FontSmall,
        ForeColor = PawTheme.TextDark,
        AutoSize = true,
        Location = loc,
        BackColor = Color.Transparent,
    };

    private static void PaintBorder(PaintEventArgs e, Control c)
    {
        using var pen = new Pen(PawTheme.CardBorder, 1.5f);
        e.Graphics.DrawRectangle(pen, 0, 0, c.Width - 1, c.Height - 1);
    }

    private void ShowInfo(string msg) =>
        MessageBox.Show(msg, "Pawductivity 🐾", MessageBoxButtons.OK, MessageBoxIcon.Information);
}