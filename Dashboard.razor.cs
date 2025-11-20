# Blazor Desktop Dashboard

## Dashboard.razor

```razor
@page "/dashboard"
@using System.Collections.Generic
@using System.Linq

<div class="dashboard-container">
    <div class="header">
        <h1>Mitglieder Dashboard</h1>
        <p>Verwalte und überwache deine Community</p>
    </div>

    <!-- Stats Grid -->
    <div class="stats-grid">
        @foreach (var stat in stats)
        {
            <div class="stat-card">
                <div class="stat-header">
                    <div class="stat-icon @stat.ColorClass">
                        <span class="icon">@stat.Icon</span>
                    </div>
                    <span class="stat-value">@stat.Value</span>
                </div>
                <h3>@stat.Label</h3>
                <p class="stat-change">@stat.Change</p>
            </div>
        }
    </div>

    <!-- Progress Card -->
    <div class="progress-card">
        <div class="progress-header">
            <div>
                <h2>@CurrentSets/@TotalSets SETS</h2>
                <p>Dein Fortschritt</p>
            </div>
            <button class="btn-primary" @onclick="GetMore">GET MORE</button>
        </div>
        <div class="progress-bar-container">
            <div class="progress-bar" style="width: @(ProgressPercentage)%"></div>
        </div>
    </div>

    <!-- Search and Filter -->
    <div class="search-filter-card">
        <div class="search-container">
            <input type="text" 
                   placeholder="Mitglieder suchen..." 
                   @bind="searchTerm" 
                   @bind:event="oninput"
                   class="search-input" />
        </div>
        <div class="filter-buttons">
            <button class="filter-btn @(filterStatus == "all" ? "active" : "")" 
                    @onclick="@(() => SetFilter("all"))">
                Alle
            </button>
            <button class="filter-btn @(filterStatus == "active" ? "active-filter" : "")" 
                    @onclick="@(() => SetFilter("active"))">
                Aktiv
            </button>
            <button class="filter-btn @(filterStatus == "pending" ? "pending-filter" : "")" 
                    @onclick="@(() => SetFilter("pending"))">
                Ausstehend
            </button>
        </div>
    </div>

    <!-- Members Grid -->
    <div class="members-grid">
        @if (FilteredMembers.Any())
        {
            @foreach (var member in FilteredMembers)
            {
                <div class="member-card">
                    <div class="member-header">
                        <span class="status-badge @(member.Status == "active" ? "status-active" : "status-pending")">
                            @(member.Status == "active" ? "Aktiv" : "Ausstehend")
                        </span>
                        <div class="member-avatar @member.ColorClass">
                            @member.Initial
                        </div>
                    </div>
                    <div class="member-body">
                        <h3>@member.Name</h3>
                        <p class="member-date">Mitglied seit @member.JoinDate</p>
                        <div class="member-contact">
                            <div class="contact-item">
                                <span class="icon">✉</span>
                                <span>@member.Email</span>
                            </div>
                            <div class="contact-item">
                                <span class="icon">📞</span>
                                <span>@member.Phone</span>
                            </div>
                        </div>
                        <button class="btn-view-profile" @onclick="@(() => ViewProfile(member.Id))">
                            Profil anzeigen
                        </button>
                    </div>
                </div>
            }
        }
        else
        {
            <div class="empty-state">
                <div class="empty-icon">👥</div>
                <h3>Keine Mitglieder gefunden</h3>
                <p>Versuche eine andere Suche oder Filter</p>
            </div>
        }
    </div>

    <!-- Footer -->
    <div class="footer-card">
        <div class="admin-info">
            <div class="admin-avatar">F</div>
            <div>
                <p class="admin-name">Friedrich Wehrmann</p>
                <p class="admin-role">Administrator</p>
            </div>
        </div>
        <button class="btn-settings" @onclick="OpenSettings">Einstellungen</button>
    </div>
</div>

@code {
    private string searchTerm = "";
    private string filterStatus = "all";
    private int CurrentSets = 4;
    private int TotalSets = 8;
    private double ProgressPercentage => (double)CurrentSets / TotalSets * 100;

    private List<StatCard> stats = new List<StatCard>
    {
        new StatCard { Icon = "👥", Label = "Gesamt Mitglieder", Value = "8", Change = "+2 diesen Monat", ColorClass = "bg-blue" },
        new StatCard { Icon = "✓", Label = "Aktive Mitglieder", Value = "4", Change = "50% Fortschritt", ColorClass = "bg-green" },
        new StatCard { Icon = "📅", Label = "Neue Anfragen", Value = "1", Change = "Diese Woche", ColorClass = "bg-purple" },
        new StatCard { Icon = "📈", Label = "Wachstum", Value = "+25%", Change = "vs. letzter Monat", ColorClass = "bg-orange" }
    };

    private List<Member> members = new List<Member>
    {
        new Member { Id = 1, Name = "Tadeusz Nitkin", Email = "tadeusz@gelfilm.de", Phone = "0163", Status = "active", JoinDate = "2024-01", Initial = "T", ColorClass = "color-purple" },
        new Member { Id = 2, Name = "Carolina Wehrmann", Email = "carolina@gelfilm.de", Phone = "0163", Status = "active", JoinDate = "2024-02", Initial = "C", ColorClass = "color-indigo" },
        new Member { Id = 3, Name = "Max Mustermann", Email = "max@gelfilm.de", Phone = "0172", Status = "active", JoinDate = "2024-03", Initial = "M", ColorClass = "color-blue" },
        new Member { Id = 4, Name = "Anna Schmidt", Email = "anna@gelfilm.de", Phone = "0171", Status = "pending", JoinDate = "2024-10", Initial = "A", ColorClass = "color-pink" }
    };

    private IEnumerable<Member> FilteredMembers => members
        .Where(m => (string.IsNullOrEmpty(searchTerm) ||
                    m.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    m.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) &&
                    (filterStatus == "all" || m.Status == filterStatus));

    private void SetFilter(string status)
    {
        filterStatus = status;
    }

    private void ViewProfile(int id)
    {
        // Implementiere Profil-Anzeige
        Console.WriteLine($"Viewing profile for member {id}");
    }

    private void GetMore()
    {
        // Implementiere "Get More" Funktionalität
        Console.WriteLine("Get More clicked");
    }

    private void OpenSettings()
    {
        // Implementiere Einstellungen
        Console.WriteLine("Settings opened");
    }

    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string JoinDate { get; set; } = string.Empty;
        public string Initial { get; set; } = string.Empty;
        public string ColorClass { get; set; } = string.Empty;
    }

    public class StatCard
    {
        public string Icon { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Change { get; set; } = string.Empty;
        public string ColorClass { get; set; } = string.Empty;
    }
}
```

## Dashboard.razor.css

```css
.dashboard-container {
    min-height: 100vh;
    background: linear-gradient(135deg, #f8fafc 0%, #e0e7ff 50%, #ede9fe 100%);
    padding: 24px;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.header {
    margin-bottom: 32px;
}

.header h1 {
    font-size: 32px;
    font-weight: bold;
    color: #1f2937;
    margin: 0 0 8px 0;
}

.header p {
    color: #6b7280;
    margin: 0;
}

.stats-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 24px;
    margin-bottom: 32px;
}

.stat-card {
    background: white;
    border-radius: 16px;
    padding: 24px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
    transition: all 0.3s;
    border: 1px solid #f3f4f6;
}

.stat-card:hover {
    transform: translateY(-4px);
    box-shadow: 0 12px 24px rgba(0, 0, 0, 0.1);
}

.stat-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 16px;
}

.stat-icon {
    width: 48px;
    height: 48px;
    border-radius: 12px;
    display: flex;
    align-items: center;
    justify-content: center;
    color: white;
    font-size: 20px;
}

.stat-icon.bg-blue { background: #3b82f6; }
.stat-icon.bg-green { background: #10b981; }
.stat-icon.bg-purple { background: #8b5cf6; }
.stat-icon.bg-orange { background: #f59e0b; }

.stat-value {
    font-size: 32px;
    font-weight: bold;
    color: #1f2937;
}

.stat-card h3 {
    font-size: 14px;
    font-weight: 500;
    color: #6b7280;
    margin: 0 0 4px 0;
}

.stat-change {
    font-size: 12px;
    color: #9ca3af;
    margin: 0;
}

.progress-card {
    background: linear-gradient(135deg, #7c3aed 0%, #4f46e5 100%);
    border-radius: 16px;
    padding: 24px;
    margin-bottom: 32px;
    color: white;
    box-shadow: 0 8px 16px rgba(124, 58, 237, 0.3);
}

.progress-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 16px;
}

.progress-header h2 {
    font-size: 28px;
    font-weight: bold;
    margin: 0 0 4px 0;
}

.progress-header p {
    color: rgba(255, 255, 255, 0.8);
    margin: 0;
}

.btn-primary {
    background: white;
    color: #7c3aed;
    border: none;
    padding: 10px 20px;
    border-radius: 8px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s;
}

.btn-primary:hover {
    background: #f5f3ff;
}

.progress-bar-container {
    background: rgba(255, 255, 255, 0.2);
    height: 12px;
    border-radius: 6px;
    overflow: hidden;
}

.progress-bar {
    background: white;
    height: 100%;
    border-radius: 6px;
    transition: width 0.5s ease;
}

.search-filter-card {
    background: white;
    border-radius: 16px;
    padding: 24px;
    margin-bottom: 24px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
    display: flex;
    gap: 16px;
    flex-wrap: wrap;
    border: 1px solid #f3f4f6;
}

.search-container {
    flex: 1;
    min-width: 200px;
}

.search-input {
    width: 100%;
    padding: 12px 16px;
    border: 1px solid #e5e7eb;
    border-radius: 12px;
    font-size: 14px;
    transition: all 0.3s;
}

.search-input:focus {
    outline: none;
    border-color: #8b5cf6;
    box-shadow: 0 0 0 3px rgba(139, 92, 246, 0.1);
}

.filter-buttons {
    display: flex;
    gap: 8px;
}

.filter-btn {
    padding: 12px 20px;
    border: none;
    border-radius: 12px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s;
    background: #f3f4f6;
    color: #6b7280;
}

.filter-btn:hover {
    background: #e5e7eb;
}

.filter-btn.active {
    background: #8b5cf6;
    color: white;
}

.filter-btn.active-filter {
    background: #10b981;
    color: white;
}

.filter-btn.pending-filter {
    background: #f59e0b;
    color: white;
}

.members-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
    gap: 24px;
    margin-bottom: 32px;
}

.member-card {
    background: white;
    border-radius: 16px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
    overflow: hidden;
    transition: all 0.3s;
    border: 1px solid #f3f4f6;
}

.member-card:hover {
    transform: translateY(-4px);
    box-shadow: 0 12px 24px rgba(0, 0, 0, 0.1);
}

.member-header {
    background: linear-gradient(135deg, #ede9fe 0%, #dbeafe 100%);
    padding: 24px;
    position: relative;
    display: flex;
    flex-direction: column;
    align-items: center;
}

.status-badge {
    position: absolute;
    top: 12px;
    right: 12px;
    padding: 4px 12px;
    border-radius: 12px;
    font-size: 12px;
    font-weight: 600;
}

.status-active {
    background: #10b981;
    color: white;
}

.status-pending {
    background: #f59e0b;
    color: white;
}

.member-avatar {
    width: 96px;
    height: 96px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    color: white;
    font-size: 36px;
    font-weight: bold;
    box-shadow: 0 8px 16px rgba(0, 0, 0, 0.15);
    transition: transform 0.3s;
}

.member-card:hover .member-avatar {
    transform: scale(1.1);
}

.member-avatar.color-purple { background: #7c3aed; }
.member-avatar.color-indigo { background: #4f46e5; }
.member-avatar.color-blue { background: #3b82f6; }
.member-avatar.color-pink { background: #ec4899; }

.member-body {
    padding: 24px;
}

.member-body h3 {
    font-size: 20px;
    font-weight: bold;
    color: #1f2937;
    margin: 0 0 4px 0;
    text-align: center;
}

.member-date {
    font-size: 12px;
    color: #9ca3af;
    text-align: center;
    margin: 0 0 16px 0;
}

.member-contact {
    margin-bottom: 16px;
}

.contact-item {
    display: flex;
    align-items: center;
    gap: 8px;
    color: #6b7280;
    font-size: 14px;
    margin-bottom: 8px;
    transition: color 0.3s;
}

.contact-item:hover {
    color: #8b5cf6;
}

.btn-view-profile {
    width: 100%;
    background: linear-gradient(135deg, #7c3aed 0%, #4f46e5 100%);
    color: white;
    border: none;
    padding: 12px;
    border-radius: 12px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s;
    box-shadow: 0 4px 8px rgba(124, 58, 237, 0.3);
}

.btn-view-profile:hover {
    transform: translateY(-2px);
    box-shadow: 0 6px 12px rgba(124, 58, 237, 0.4);
}

.empty-state {
    grid-column: 1 / -1;
    background: white;
    border-radius: 16px;
    padding: 48px;
    text-align: center;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
    border: 1px solid #f3f4f6;
}

.empty-icon {
    font-size: 64px;
    margin-bottom: 16px;
}

.empty-state h3 {
    font-size: 20px;
    font-weight: 600;
    color: #1f2937;
    margin: 0 0 8px 0;
}

.empty-state p {
    color: #9ca3af;
    margin: 0;
}

.footer-card {
    background: white;
    border-radius: 16px;
    padding: 16px 24px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
    display: flex;
    justify-content: space-between;
    align-items: center;
    border: 1px solid #f3f4f6;
}

.admin-info {
    display: flex;
    align-items: center;
    gap: 12px;
}

.admin-avatar {
    width: 40px;
    height: 40px;
    border-radius: 50%;
    background: linear-gradient(135deg, #7c3aed 0%, #4f46e5 100%);
    color: white;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: bold;
}

.admin-name {
    font-weight: 600;
    color: #1f2937;
    margin: 0;
}

.admin-role {
    font-size: 12px;
    color: #9ca3af;
    margin: 0;
}

.btn-settings {
    background: transparent;
    color: #8b5cf6;
    border: none;
    font-weight: 600;
    cursor: pointer;
    padding: 8px 16px;
    border-radius: 8px;
    transition: all 0.3s;
}

.btn-settings:hover {
    background: #f5f3ff;
}
```

## So verwendest du das Dashboard in deinem Blazor-Projekt:

1. Erstelle die Datei `Dashboard.razor` im `Pages` Ordner
2. Erstelle die Datei `Dashboard.razor.css` im selben Ordner (optional, für CSS Isolation)
3. Füge in `_Imports.razor` hinzu:
   ```razor
   @using System.Linq
   ```

4. Für Navigation in `NavMenu.razor`:
   ```razor
   <div class="nav-item px-3">
       <NavLink class="nav-link" href="dashboard">
           <span class="oi oi-dashboard" aria-hidden="true"></span> Dashboard
       </NavLink>
   </div>
   ```

Die Komponente ist vollständig einsatzbereit für Blazor Server oder Blazor WebAssembly!