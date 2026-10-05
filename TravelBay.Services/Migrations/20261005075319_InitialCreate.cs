using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TravelBay.Services.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Base64Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IconName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ProfileImageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Assets_ProfileImageId",
                        column: x => x.ProfileImageId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PerformedByUserId = table.Column<int>(type: "int", nullable: true),
                    PerformedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_PerformedByUserId",
                        column: x => x.PerformedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Collections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TripPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripPlans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreferences_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    DateAssigned = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Destinations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    Keywords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Destinations_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Destinations_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CollectionItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    DestinationId = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionItems_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionItems_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DestinationImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DestinationId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    IsAiGenerated = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestinationImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DestinationImages_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DestinationId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModeratedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModeratedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModerationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_ModeratedByUserId",
                        column: x => x.ModeratedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SavedDestinations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DestinationId = table.Column<int>(type: "int", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedDestinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedDestinations_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SavedDestinations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TripPlanItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TripPlanId = table.Column<int>(type: "int", nullable: false),
                    DestinationId = table.Column<int>(type: "int", nullable: false),
                    DayNumber = table.Column<int>(type: "int", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripPlanItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripPlanItems_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TripPlanItems_TripPlans_TripPlanId",
                        column: x => x.TripPlanId,
                        principalTable: "TripPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ViewHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DestinationId = table.Column<int>(type: "int", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViewHistories_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ViewHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "IconName", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "beach", true, "Plaže", null },
                    { 2, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "mountain", true, "Planine", null },
                    { 3, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "landmark", true, "Historija", null },
                    { 4, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "restaurant", true, "Hrana", null },
                    { 5, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "nature", true, "Priroda", null }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Bosna i Hercegovina" },
                    { 2, "Hrvatska" },
                    { 3, "Crna Gora" }
                });

            migrationBuilder.InsertData(
                table: "News",
                columns: new[] { "Id", "Content", "CreatedAt", "ImageUrl", "Title" },
                values: new object[,]
                {
                    { 1, "Grad Sarajevo pokreće tri nove pješačke ture kroz historijsko jezgro grada.", new DateTime(2026, 3, 4, 0, 0, 0, 0, DateTimeKind.Utc), "https://picsum.photos/seed/travelbay-news-1/800/400", "Sarajevo uvodi nove turističke rute" },
                    { 2, "Hrvatska i Crna Gora bilježe rekordan broj turista ove ljetne sezone.", new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), "https://picsum.photos/seed/travelbay-news-2/800/400", "Rekordna sezona na Jadranu" },
                    { 3, "NP Una i NP Sutjeska proširuju mrežu markiranih planinarskih staza.", new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), "https://picsum.photos/seed/travelbay-news-3/800/400", "Nacionalni parkovi otvaraju nove staze" },
                    { 4, "Hercegovačka kulinarska tura ugostit će regionalni gastro festival.", new DateTime(2026, 3, 7, 0, 0, 0, 0, DateTimeKind.Utc), "https://picsum.photos/seed/travelbay-news-4/800/400", "Gastronomski festival u Mostaru" },
                    { 5, "Novi sistem preporuka pomaže korisnicima da pronađu destinacije po mjeri.", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), "https://picsum.photos/seed/travelbay-news-5/800/400", "TravelBay predstavlja preporuke putovanja" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Administrator role with full permissions", true, "Admin" },
                    { 2, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Regular TravelBay user", true, "User" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "LastLoginAt", "LastName", "PasswordHash", "PasswordSalt", "PhoneNumber", "ProfileImageId", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "desktop@travelbay.local", "Desktop", true, null, "Admin", "WdE1iyTU37rDUJ6ZVWm+IVeaoZQ=", "/vKjmkkerpD5Gm0uqqm6lQ==", null, null, "desktop" },
                    { 2, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "mobile@travelbay.local", "Mobile", true, null, "User", "OH3vkoGdPG5KiEvWJuJu0AUTH7w=", "7wPk2p3d/6zQEauDvWjVvQ==", null, null, "mobile" },
                    { 3, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "amina.hodzic@example.com", "Amina", true, null, "Hodžić", "+o3/xYezJVCJCsHOCG+7gBCSXsQ=", "omcADDd4EkMqIHOQ+0KouQ==", null, null, "amina" },
                    { 4, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "marko.juric@example.com", "Marko", true, null, "Jurić", "JZZOw5JAp986X1xigQcJz4D2qUI=", "y740EVXQuhm68kDp+dp4KA==", null, null, "marko" },
                    { 5, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "elena.petrovic@example.com", "Elena", true, null, "Petrović", "y/ML2lIEeQ2pXrL6J6JgjgsL+5Y=", "/y/XeyiPaksiaR5Rqgp7GQ==", null, null, "elena" },
                    { 6, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "ivan.babic@example.com", "Ivan", true, null, "Babić", "hcUZJx0ppieMdpbFlffeg2Ztoos=", "nY6lNc/fQh7jV/v1zo4q3Q==", null, null, "ivan" },
                    { 7, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "lara.vukovic@example.com", "Lara", true, null, "Vuković", "cahUhwK0z7Ll5sCJ4Yiiv571THI=", "xFbTotl77sSQ3ejO04Ykrw==", null, null, "lara" }
                });

            migrationBuilder.InsertData(
                table: "AuditLogs",
                columns: new[] { "Id", "Action", "Details", "EntityId", "EntityName", "PerformedAt", "PerformedByUserId" },
                values: new object[,]
                {
                    { 1, "Approved", "Review for destination 1 approved.", 1, "Review", new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, "Approved", "Review for destination 2 approved.", 3, "Review", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 3, "Rejected", "Review for destination 13 rejected: neprimjeren sadržaj.", 19, "Review", new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 4, "Rejected", "Review for destination 20 rejected: neprimjeren sadržaj.", 26, "Review", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 5, "StatusChanged:Active", "Trip plan 'Ljeto na Jadranu' moved to Active.", 2, "TripPlan", new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { 6, "StatusChanged:Completed", "Trip plan 'Sedmica u planinama BiH' moved to Completed.", 3, "TripPlan", new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Utc), 5 },
                    { 7, "StatusChanged:Cancelled", "Trip plan 'Istraživanje nacionalnih parkova' moved to Cancelled.", 4, "TripPlan", new DateTime(2026, 2, 8, 0, 0, 0, 0, DateTimeKind.Utc), 6 },
                    { 8, "StatusChanged:Active", "Trip plan 'Kulturni obilazak regije' moved to Active.", 5, "TripPlan", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7 }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "CountryId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Sarajevo" },
                    { 2, 1, "Mostar" },
                    { 3, 1, "Trebinje" },
                    { 4, 1, "Neum" },
                    { 5, 1, "Jajce" },
                    { 6, 1, "Konjic" },
                    { 7, 1, "Bihać" },
                    { 8, 1, "Foča" },
                    { 9, 1, "Ljubuški" },
                    { 10, 1, "Blagaj" },
                    { 11, 2, "Dubrovnik" },
                    { 12, 2, "Split" },
                    { 13, 3, "Kotor" },
                    { 14, 3, "Budva" },
                    { 15, 3, "Perast" }
                });

            migrationBuilder.InsertData(
                table: "Collections",
                columns: new[] { "Id", "CreatedAt", "Name", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Omiljena mjesta", 3 },
                    { 2, new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Za sljedeće ljeto", 4 },
                    { 3, new DateTime(2026, 1, 27, 0, 0, 0, 0, DateTimeKind.Utc), "Planinski izleti", 5 },
                    { 4, new DateTime(2026, 1, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Historijske znamenitosti", 6 },
                    { 5, new DateTime(2026, 1, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Plaže za opuštanje", 7 },
                    { 6, new DateTime(2026, 1, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Mobile test kolekcija", 2 }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "IsRead", "Message", "Title", "Type", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, "Vaša recenzija za Stari Most je odobrena.", "Recenzija odobrena", 0, 3 },
                    { 2, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Utc), false, "Vaša recenzija za Vrelo Bune nije odobrena.", "Recenzija odbijena", 1, 6 },
                    { 3, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Vaša recenzija za Plažu Neum nije odobrena.", "Recenzija odbijena", 1, 3 },
                    { 4, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Utc), true, "Plan 'Ljeto na Jadranu' je sada aktivan.", "Status plana putovanja promijenjen", 2, 4 },
                    { 5, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Utc), true, "Plan 'Sedmica u planinama BiH' je završen.", "Status plana putovanja promijenjen", 2, 5 },
                    { 6, new DateTime(2026, 2, 8, 0, 0, 0, 0, DateTimeKind.Utc), false, "Plan 'Istraživanje nacionalnih parkova' je otkazan.", "Status plana putovanja promijenjen", 2, 6 },
                    { 7, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), false, "Pročitajte najnovije vijesti o turizmu u regiji.", "Nova vijest na TravelBayu", 3, 3 },
                    { 8, new DateTime(2026, 3, 8, 0, 0, 0, 0, DateTimeKind.Utc), false, "Pogledajte preporuke za jesenja putovanja.", "Nova vijest na TravelBayu", 3, 4 },
                    { 9, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), true, "Hvala što ste se pridružili TravelBay zajednici.", "Dobrodošli na TravelBay", 4, 7 },
                    { 10, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), true, "Hvala što ste se pridružili TravelBay zajednici.", "Dobrodošli na TravelBay", 4, 2 }
                });

            migrationBuilder.InsertData(
                table: "TripPlans",
                columns: new[] { "Id", "CreatedAt", "EndDate", "IsDeleted", "Name", "StartDate", "Status", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 14, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Utc), false, "Vikend u Mostaru i Hercegovini", new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, 3 },
                    { 2, new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 23, 0, 0, 0, 0, DateTimeKind.Utc), false, "Ljeto na Jadranu", new DateTime(2026, 3, 16, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { 3, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Sedmica u planinama BiH", new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc), 2, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Utc), 5 },
                    { 4, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 14, 0, 0, 0, 0, DateTimeKind.Utc), false, "Istraživanje nacionalnih parkova", new DateTime(2026, 2, 9, 0, 0, 0, 0, DateTimeKind.Utc), 3, new DateTime(2026, 2, 8, 0, 0, 0, 0, DateTimeKind.Utc), 6 },
                    { 5, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 11, 0, 0, 0, 0, DateTimeKind.Utc), false, "Kulturni obilazak regije", new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 7 },
                    { 6, new DateTime(2026, 3, 26, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 7, 0, 0, 0, 0, DateTimeKind.Utc), false, "Produženi vikend Sarajevo–Mostar", new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, 3 }
                });

            migrationBuilder.InsertData(
                table: "UserPreferences",
                columns: new[] { "Id", "CategoryId", "UserId" },
                values: new object[,]
                {
                    { 1, 3, 3 },
                    { 2, 5, 3 },
                    { 3, 1, 4 },
                    { 4, 4, 4 },
                    { 5, 2, 5 },
                    { 6, 5, 5 },
                    { 7, 3, 5 },
                    { 8, 3, 6 },
                    { 9, 4, 6 },
                    { 10, 1, 7 },
                    { 11, 2, 7 },
                    { 12, 5, 7 }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "DateAssigned", "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1 },
                    { 2, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 2, 2 },
                    { 3, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 2, 3 },
                    { 4, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 2, 4 },
                    { 5, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 2, 5 },
                    { 6, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 2, 6 },
                    { 7, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 2, 7 }
                });

            migrationBuilder.InsertData(
                table: "Destinations",
                columns: new[] { "Id", "CategoryId", "CityId", "CreatedAt", "Description", "IsDeleted", "Keywords", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 3, 2, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Ikonični osmanski kameni most u Mostaru iz 16. vijeka, obnovljen 2004. godine i upisan na UNESCO-vu listu svjetske baštine.", false, "Stari Most, historija, izlet, odmor", "Stari Most", null },
                    { 2, 3, 1, new DateTime(2026, 1, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Historijska trgovačka četvrt Sarajeva s brojnim zanatskim radnjama, džamijama i Sebiljem u srcu grada.", false, "Baščaršija, historija, izlet, odmor", "Baščaršija", null },
                    { 3, 3, 2, new DateTime(2026, 1, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Srednjovjekovno utvrđeno naselje iznad rijeke Neretve, poznato po kuli Hamam i kamenoj arhitekturi.", false, null, "Počitelj", null },
                    { 4, 3, 5, new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Srednjovjekovna tvrđava i posljednja prijestonica bosanskih kraljeva, smještena iznad vodopada Pliva.", false, null, "Tvrđava Jajce", null },
                    { 5, 3, 3, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Osmanska stara čaršija na obalama rijeke Trebišnjice, poznata po Arslanagića mostu u blizini.", false, "Stari grad Trebinje, historija, izlet, odmor", "Stari grad Trebinje", null },
                    { 6, 3, 11, new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Srednjovjekovni grad opasan zidinama, jedan od najbolje očuvanih utvrđenih gradova na Mediteranu.", false, "Stari grad Dubrovnik, historija, izlet, odmor", "Stari grad Dubrovnik", null },
                    { 7, 2, 1, new DateTime(2026, 1, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Planina iznad Sarajeva, olimpijsko skijalište iz 1984. godine s panoramskim pogledom na okolne vrhove.", false, null, "Bjelašnica", null },
                    { 8, 2, 1, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Najviše i najizolovanije stalno naseljeno selo u BiH, poznato po očuvanoj tradicionalnoj arhitekturi.", false, null, "Lukomir", null },
                    { 9, 2, 3, new DateTime(2026, 1, 24, 0, 0, 0, 0, DateTimeKind.Utc), "Najjužniji planinski masiv Dinarida s vrhovima iznad 1800m, blizu granice s Crnom Gorom.", false, null, "Orjen", null },
                    { 10, 5, 1, new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Izvorište rijeke Bosne u podnožju Igmana, uređeni park s alejama i jezercima — omiljeno izletište Sarajeva.", false, "Vrelo Bosne, priroda, izlet, odmor", "Vrelo Bosne", null },
                    { 11, 5, 9, new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Tufasti vodopad na rijeci Trebižat visine oko 25m, okružen prirodnim amfiteatrom i jezerom pogodnim za kupanje.", false, "Vodopadi Kravica, priroda, izlet, odmor", "Vodopadi Kravica", null },
                    { 12, 5, 7, new DateTime(2026, 1, 27, 0, 0, 0, 0, DateTimeKind.Utc), "Nacionalni park oko rijeke Une poznate po bistroj vodi, brzacima i slapovima Štrbački buk.", false, null, "Nacionalni park Una", null },
                    { 13, 5, 10, new DateTime(2026, 1, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Jedan od najjačih krških izvora u Evropi, uz koji se nalazi derviška tekija iz 16. vijeka podno litice.", false, "Vrelo Bune (Blagaj), priroda, izlet, odmor", "Vrelo Bune (Blagaj)", null },
                    { 14, 5, 8, new DateTime(2026, 1, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Najstariji nacionalni park u BiH s prašumom Perućica i najvišim vrhom Maglić (2386m).", false, null, "Nacionalni park Sutjeska", null },
                    { 15, 5, 6, new DateTime(2026, 1, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Glacijalno jezero okruženo šumovitim planinama u blizini Konjica, pogodno za kupanje i rekreaciju.", false, null, "Boračko jezero", null },
                    { 16, 4, 1, new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Utc), "Tradicionalne sarajevske ćevabdžinice u srcu Baščaršije, poznate po ćevapima u somunu.", false, "Ćevabdžinice Baščaršije, gastronomija, izlet, odmor", "Ćevabdžinice Baščaršije", null },
                    { 17, 4, 3, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Hercegovačka vinska regija oko Trebinja poznata po žilavki i vranacu.", false, null, "Vinski podrumi Trebinja", null },
                    { 18, 4, 4, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Riblji restorani duž jedine bosanskohercegovačke obale, poznati po svježim plodovima mora.", false, null, "Neum — morski specijaliteti", null },
                    { 19, 4, 2, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Degustacija lokalnih specijaliteta Mostara i okoline — suhog mesa, sira i domaćeg vina.", false, null, "Hercegovačka kulinarska tura", null },
                    { 20, 1, 4, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Glavna plaža jedinog bosanskohercegovačkog izlaza na more, s pogledom na Pelješki kanal.", false, "Plaža Neum, plaža, izlet, odmor", "Plaža Neum", null },
                    { 21, 1, 11, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Gradska plaža Dubrovnika s pogledom na stare gradske zidine i ostrvo Lokrum.", false, "Plaža Banje, plaža, izlet, odmor", "Plaža Banje", null },
                    { 22, 1, 12, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Najpoznatija splitska plaža u obliku školjke, poznata po igri 'picigin'.", false, null, "Bačvice", null },
                    { 23, 1, 14, new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), "Dvije uvale pješčano-šljunčane plaže ispod stare budvanske tvrđave.", false, null, "Plaža Mogren", null },
                    { 24, 1, 15, new DateTime(2026, 2, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Umjetno ostrvo ispred Perasta s baroknom crkvom, nastalo nasipanjem kamena i potapanjem starih brodova.", false, "Gospa od Škrpjela, plaža, izlet, odmor", "Gospa od Škrpjela", null }
                });

            migrationBuilder.InsertData(
                table: "CollectionItems",
                columns: new[] { "Id", "AddedAt", "CollectionId", "DestinationId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2 },
                    { 2, new DateTime(2026, 1, 26, 1, 0, 0, 0, DateTimeKind.Utc), 1, 6 },
                    { 3, new DateTime(2026, 1, 26, 2, 0, 0, 0, DateTimeKind.Utc), 1, 13 },
                    { 4, new DateTime(2026, 1, 27, 0, 0, 0, 0, DateTimeKind.Utc), 2, 24 },
                    { 5, new DateTime(2026, 1, 27, 1, 0, 0, 0, DateTimeKind.Utc), 2, 7 },
                    { 6, new DateTime(2026, 1, 27, 2, 0, 0, 0, DateTimeKind.Utc), 2, 8 },
                    { 7, new DateTime(2026, 1, 28, 0, 0, 0, 0, DateTimeKind.Utc), 3, 15 },
                    { 8, new DateTime(2026, 1, 28, 1, 0, 0, 0, DateTimeKind.Utc), 3, 9 },
                    { 9, new DateTime(2026, 1, 28, 2, 0, 0, 0, DateTimeKind.Utc), 3, 1 },
                    { 10, new DateTime(2026, 1, 29, 0, 0, 0, 0, DateTimeKind.Utc), 4, 3 },
                    { 11, new DateTime(2026, 1, 29, 1, 0, 0, 0, DateTimeKind.Utc), 4, 4 },
                    { 12, new DateTime(2026, 1, 29, 2, 0, 0, 0, DateTimeKind.Utc), 4, 5 },
                    { 13, new DateTime(2026, 1, 30, 0, 0, 0, 0, DateTimeKind.Utc), 5, 20 },
                    { 14, new DateTime(2026, 1, 30, 1, 0, 0, 0, DateTimeKind.Utc), 5, 21 },
                    { 15, new DateTime(2026, 1, 30, 2, 0, 0, 0, DateTimeKind.Utc), 5, 22 },
                    { 16, new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Utc), 6, 23 },
                    { 17, new DateTime(2026, 1, 31, 1, 0, 0, 0, DateTimeKind.Utc), 6, 10 },
                    { 18, new DateTime(2026, 1, 31, 2, 0, 0, 0, DateTimeKind.Utc), 6, 11 }
                });

            migrationBuilder.InsertData(
                table: "DestinationImages",
                columns: new[] { "Id", "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "OrderIndex", "Source" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), 1, "https://picsum.photos/seed/travelbay-1/800/600", false, 0, "Seed data" },
                    { 2, new DateTime(2026, 1, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "https://picsum.photos/seed/travelbay-2/800/600", false, 0, "Seed data" },
                    { 3, new DateTime(2026, 1, 18, 0, 0, 0, 0, DateTimeKind.Utc), 3, "https://picsum.photos/seed/travelbay-3/800/600", false, 0, "Seed data" },
                    { 4, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), 5, "https://picsum.photos/seed/travelbay-5/800/600", false, 0, "Seed data" },
                    { 5, new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), 6, "https://picsum.photos/seed/travelbay-6/800/600", false, 0, "Seed data" },
                    { 6, new DateTime(2026, 1, 22, 0, 0, 0, 0, DateTimeKind.Utc), 7, "https://picsum.photos/seed/travelbay-7/800/600", false, 0, "Seed data" },
                    { 7, new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc), 10, "https://picsum.photos/seed/travelbay-10/800/600", false, 0, "Seed data" },
                    { 8, new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc), 11, "https://picsum.photos/seed/travelbay-11/800/600", false, 0, "Seed data" },
                    { 9, new DateTime(2026, 1, 27, 0, 0, 0, 0, DateTimeKind.Utc), 12, "https://picsum.photos/seed/travelbay-12/800/600", false, 0, "Seed data" },
                    { 10, new DateTime(2026, 1, 28, 0, 0, 0, 0, DateTimeKind.Utc), 13, "https://picsum.photos/seed/travelbay-13/800/600", false, 0, "Seed data" },
                    { 11, new DateTime(2026, 1, 30, 0, 0, 0, 0, DateTimeKind.Utc), 15, "https://picsum.photos/seed/travelbay-15/800/600", false, 0, "Seed data" },
                    { 12, new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Utc), 16, "https://picsum.photos/seed/travelbay-16/800/600", false, 0, "Seed data" },
                    { 13, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Utc), 19, "https://picsum.photos/seed/travelbay-19/800/600", false, 0, "Seed data" },
                    { 14, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Utc), 20, "https://picsum.photos/seed/travelbay-20/800/600", false, 0, "Seed data" },
                    { 15, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), 21, "https://picsum.photos/seed/travelbay-21/800/600", false, 0, "Seed data" },
                    { 16, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Utc), 22, "https://picsum.photos/seed/travelbay-22/800/600", false, 0, "Seed data" },
                    { 17, new DateTime(2026, 2, 8, 0, 0, 0, 0, DateTimeKind.Utc), 24, "https://picsum.photos/seed/travelbay-24/800/600", false, 0, "Seed data" }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "Comment", "CreatedAt", "DestinationId", "IsDeleted", "ModeratedAt", "ModeratedByUserId", "ModerationReason", "Rating", "Status", "UserId" },
                values: new object[,]
                {
                    { 1, "Prelijep most, obavezno posjetiti u zoru dok nema gužve.", new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 5, 1, 3 },
                    { 2, "Impresivan prizor, ali dosta turista ljeti.", new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 4, 1, 4 },
                    { 3, "Baščaršija ima poseban šarm, hrana odlična.", new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Utc), 2, false, new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 5, 1, 5 },
                    { 4, "Puno radnji sa suvenirima, preporučujem šetnju uveče.", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), 2, false, new DateTime(2026, 2, 8, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 4, 1, 6 },
                    { 5, "Počitelj izgleda kao iz bajke, vrijedi penjanje do kule.", new DateTime(2026, 2, 8, 0, 0, 0, 0, DateTimeKind.Utc), 3, false, new DateTime(2026, 2, 9, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 5, 1, 7 },
                    { 6, "Mirna čaršija, lijep pogled na Trebišnjicu.", new DateTime(2026, 2, 9, 0, 0, 0, 0, DateTimeKind.Utc), 5, false, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 4, 1, 3 },
                    { 7, "Zidine Dubrovnika su must-see, cijena ulaznice malo visoka.", new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), 6, false, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 5, 1, 4 },
                    { 8, "Nevjerovatna arhitektura, ali ljeti pretrpano.", new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Utc), 6, false, null, null, null, 5, 0, 5 },
                    { 9, "Odličan planinski zrak i staze za hiking.", new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Utc), 7, false, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 4, 1, 6 },
                    { 10, "Infrastruktura bi mogla biti bolja van sezone.", new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Utc), 7, false, new DateTime(2026, 2, 14, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 3, 1, 7 },
                    { 11, "Lukomir je kao putovanje kroz vrijeme, nezaboravno.", new DateTime(2026, 2, 14, 0, 0, 0, 0, DateTimeKind.Utc), 8, false, new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 5, 1, 3 },
                    { 12, "Orjen je nepravedno zapostavljen, divni vidici.", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 9, false, null, null, null, 4, 0, 4 },
                    { 13, "Vrelo Bosne idealno za porodični izlet.", new DateTime(2026, 2, 16, 0, 0, 0, 0, DateTimeKind.Utc), 10, false, new DateTime(2026, 2, 17, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 5, 1, 5 },
                    { 14, "Lijepo uređeno, puno biciklista i trkača.", new DateTime(2026, 2, 17, 0, 0, 0, 0, DateTimeKind.Utc), 10, false, new DateTime(2026, 2, 18, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 4, 1, 6 },
                    { 15, "Kravica je spektakularna, vodopad oduzima dah.", new DateTime(2026, 2, 18, 0, 0, 0, 0, DateTimeKind.Utc), 11, false, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 5, 1, 7 },
                    { 16, "Odlično za kupanje ljeti, dođite rano zbog gužve.", new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Utc), 11, false, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 5, 1, 3 },
                    { 17, "Rafting na Uni je top iskustvo.", new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), 12, false, new DateTime(2026, 2, 21, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 4, 1, 4 },
                    { 18, "Vrelo Bune i tekija su magično mjesto.", new DateTime(2026, 2, 21, 0, 0, 0, 0, DateTimeKind.Utc), 13, false, new DateTime(2026, 2, 22, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 5, 1, 5 },
                    { 19, "Previše turista odjednom, teško uživati u miru.", new DateTime(2026, 2, 22, 0, 0, 0, 0, DateTimeKind.Utc), 13, false, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Neprimjeren ili netačan sadržaj recenzije.", 2, 2, 6 },
                    { 20, "Boračko jezero prelijepo za kupanje i piknik.", new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Utc), 15, false, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 4, 1, 7 },
                    { 21, "Najbolji ćevapi koje sam probala, apsolutno vrijedi.", new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Utc), 16, false, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 5, 1, 3 },
                    { 22, "Autentičan sarajevski doživljaj.", new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Utc), 16, false, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 5, 1, 4 },
                    { 23, "Hrana dobra ali skupo za ono što dobijete.", new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Utc), 18, false, null, null, null, 3, 0, 5 },
                    { 24, "Vino i suho meso odlični, preporučujem turu.", new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Utc), 19, false, new DateTime(2026, 2, 28, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 4, 1, 6 },
                    { 25, "Jedina naša plaža, uvijek lijepa za vikend.", new DateTime(2026, 2, 28, 0, 0, 0, 0, DateTimeKind.Utc), 20, false, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 4, 1, 7 },
                    { 26, "Previše izgrađeno, izgubio šarm.", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), 20, false, new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Neprimjeren ili netačan sadržaj recenzije.", 1, 2, 3 },
                    { 27, "Plaža Banje s pogledom na zidine — nezaboravno.", new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Utc), 21, false, new DateTime(2026, 3, 3, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 5, 1, 4 },
                    { 28, "Skupo za ležaljke, ali pogled to opravdava.", new DateTime(2026, 3, 3, 0, 0, 0, 0, DateTimeKind.Utc), 21, false, new DateTime(2026, 3, 4, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 4, 1, 5 },
                    { 29, "Bačvice su institucija, obavezno probati picigin.", new DateTime(2026, 3, 4, 0, 0, 0, 0, DateTimeKind.Utc), 22, false, new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 5, 1, 6 },
                    { 30, "Gospa od Škrpjela je kao razglednica uživo.", new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), 24, false, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 5, 1, 7 },
                    { 31, "Vožnja brodićem do ostrva je doživljaj za sebe.", new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), 24, false, null, null, null, 4, 0, 3 }
                });

            migrationBuilder.InsertData(
                table: "SavedDestinations",
                columns: new[] { "Id", "DestinationId", "SavedAt", "UserId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 2, 2, new DateTime(2026, 1, 22, 0, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 3, 10, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 4, 11, new DateTime(2026, 1, 24, 0, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 5, 13, new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { 6, 16, new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { 7, 20, new DateTime(2026, 1, 27, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { 8, 21, new DateTime(2026, 1, 28, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { 9, 24, new DateTime(2026, 1, 29, 0, 0, 0, 0, DateTimeKind.Utc), 5 },
                    { 10, 6, new DateTime(2026, 1, 30, 0, 0, 0, 0, DateTimeKind.Utc), 5 },
                    { 11, 7, new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Utc), 5 },
                    { 12, 15, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5 },
                    { 13, 8, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Utc), 6 },
                    { 14, 9, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Utc), 6 },
                    { 15, 3, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Utc), 6 },
                    { 16, 22, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), 6 },
                    { 17, 5, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Utc), 7 },
                    { 18, 17, new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), 7 },
                    { 19, 12, new DateTime(2026, 2, 8, 0, 0, 0, 0, DateTimeKind.Utc), 7 },
                    { 20, 23, new DateTime(2026, 2, 9, 0, 0, 0, 0, DateTimeKind.Utc), 7 }
                });

            migrationBuilder.InsertData(
                table: "TripPlanItems",
                columns: new[] { "Id", "DayNumber", "DestinationId", "Notes", "OrderIndex", "TripPlanId" },
                values: new object[,]
                {
                    { 1, 1, 1, null, 1, 1 },
                    { 2, 2, 2, null, 2, 1 },
                    { 3, 3, 10, null, 3, 1 },
                    { 4, 1, 7, null, 1, 2 },
                    { 5, 2, 13, null, 2, 2 },
                    { 6, 3, 11, null, 3, 2 },
                    { 7, 1, 6, null, 1, 3 },
                    { 8, 2, 21, null, 2, 3 },
                    { 9, 3, 15, null, 3, 3 },
                    { 10, 1, 8, null, 1, 4 },
                    { 11, 2, 16, null, 2, 4 },
                    { 12, 3, 24, null, 3, 4 },
                    { 13, 1, 3, null, 1, 5 },
                    { 14, 2, 12, null, 2, 5 },
                    { 15, 3, 20, null, 3, 5 },
                    { 16, 1, 5, null, 1, 6 },
                    { 17, 2, 17, null, 2, 6 },
                    { 18, 3, 9, null, 3, 6 }
                });

            migrationBuilder.InsertData(
                table: "ViewHistories",
                columns: new[] { "Id", "DestinationId", "UserId", "ViewedAt" },
                values: new object[,]
                {
                    { 1, 1, 3, new DateTime(2026, 1, 16, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 2, 3, new DateTime(2026, 1, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 3, 3, new DateTime(2026, 1, 17, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 5, 3, new DateTime(2026, 1, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 6, 3, new DateTime(2026, 1, 18, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, 7, 3, new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, 8, 3, new DateTime(2026, 1, 19, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, 9, 4, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, 10, 4, new DateTime(2026, 1, 20, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, 11, 4, new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, 12, 4, new DateTime(2026, 1, 21, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, 13, 4, new DateTime(2026, 1, 22, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, 15, 4, new DateTime(2026, 1, 22, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, 16, 4, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, 17, 5, new DateTime(2026, 1, 23, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, 18, 5, new DateTime(2026, 1, 24, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, 19, 5, new DateTime(2026, 1, 24, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, 20, 5, new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, 21, 5, new DateTime(2026, 1, 25, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, 22, 5, new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, 24, 5, new DateTime(2026, 1, 26, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, 1, 6, new DateTime(2026, 1, 27, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, 2, 6, new DateTime(2026, 1, 27, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, 3, 6, new DateTime(2026, 1, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 25, 5, 6, new DateTime(2026, 1, 28, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 26, 6, 6, new DateTime(2026, 1, 29, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 27, 7, 6, new DateTime(2026, 1, 29, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 28, 8, 6, new DateTime(2026, 1, 30, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 29, 9, 7, new DateTime(2026, 1, 30, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 30, 10, 7, new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 31, 11, 7, new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 32, 12, 7, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 33, 13, 7, new DateTime(2026, 2, 1, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 34, 15, 7, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 35, 16, 7, new DateTime(2026, 2, 2, 12, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_PerformedByUserId",
                table: "AuditLogs",
                column: "PerformedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionItems_CollectionId",
                table: "CollectionItems",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionItems_DestinationId",
                table: "CollectionItems",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_UserId",
                table: "Collections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DestinationImages_DestinationId",
                table: "DestinationImages",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Destinations_CategoryId",
                table: "Destinations",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Destinations_CityId",
                table: "Destinations",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_DestinationId",
                table: "Reviews",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ModeratedByUserId",
                table: "Reviews",
                column: "ModeratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedDestinations_DestinationId",
                table: "SavedDestinations",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedDestinations_UserId_DestinationId",
                table: "SavedDestinations",
                columns: new[] { "UserId", "DestinationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TripPlanItems_DestinationId",
                table: "TripPlanItems",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_TripPlanItems_TripPlanId",
                table: "TripPlanItems",
                column: "TripPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TripPlans_UserId",
                table: "TripPlans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_CategoryId",
                table: "UserPreferences",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_UserId_CategoryId",
                table: "UserPreferences",
                columns: new[] { "UserId", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProfileImageId",
                table: "Users",
                column: "ProfileImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ViewHistories_DestinationId",
                table: "ViewHistories",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewHistories_UserId",
                table: "ViewHistories",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "CollectionItems");

            migrationBuilder.DropTable(
                name: "DestinationImages");

            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "SavedDestinations");

            migrationBuilder.DropTable(
                name: "TripPlanItems");

            migrationBuilder.DropTable(
                name: "UserPreferences");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "ViewHistories");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "TripPlans");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Destinations");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
