using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TravelBay.Services.Migrations
{
    /// <inheritdoc />
    public partial class SeedWikimediaImagesKeywordsDemoData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AuditLogs",
                columns: new[] { "Id", "Action", "Details", "EntityId", "EntityName", "PerformedAt", "PerformedByUserId" },
                values: new object[,]
                {
                    { 9, "Approved", "Review for destination 11 approved.", 32, "Review", new DateTime(2026, 3, 8, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 10, "StatusChanged:Active", "Trip plan 'Zima u Hercegovini' moved to Active.", 7, "TripPlan", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 11, "StatusChanged:Completed", "Trip plan 'Vikend na Uni' moved to Completed.", 8, "TripPlan", new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), 2 }
                });

            migrationBuilder.UpdateData(
                table: "Collections",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Moja lista želja");

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { "https://thumb.wikimedia.org/wikipedia/commons/thumb/d/d7/Mostar_Old_Town_Panorama_2007.jpg/1280px-Mostar_Old_Town_Panorama_2007.jpg", true, "Wikimedia Commons · CC BY-SA 4.0 · Ramirez" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { "https://thumb.wikimedia.org/wikipedia/commons/thumb/c/c5/Ba%C5%A1%C4%8Dar%C5%A1ija_2006.jpg/1280px-Ba%C5%A1%C4%8Dar%C5%A1ija_2006.jpg", true, "Wikimedia Commons · Public domain · en:User:Dark Apostrophe" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { "https://thumb.wikimedia.org/wikipedia/commons/thumb/7/7f/Pocitelj%2C_Capljina.JPG/1280px-Pocitelj%2C_Capljina.JPG", true, "Wikimedia Commons · CC BY 3.0 rs · korisnik srpske Wikipedije" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), 4, "https://thumb.wikimedia.org/wikipedia/commons/thumb/2/2f/Jajce_Tvrdava.JPG/1280px-Jajce_Tvrdava.JPG", true, "Wikimedia Commons · CC BY-SA 4.0 · Dans" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), 5, "https://thumb.wikimedia.org/wikipedia/commons/thumb/2/20/Trebinje_Stari_grad.jpg/1280px-Trebinje_Stari_grad.jpg", true, "Wikimedia Commons · CC BY-SA 4.0 · ER photography" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), 6, "https://thumb.wikimedia.org/wikipedia/commons/thumb/a/a3/Dubrovnik%2C_stari_grad.jpg/1280px-Dubrovnik%2C_stari_grad.jpg", true, "Wikimedia Commons · CC BY-SA 4.0 · Marko Jukić -Majkl" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 22, 0, 0, 0, 0, DateTimeKind.Utc), 7, "https://thumb.wikimedia.org/wikipedia/commons/thumb/8/8a/Bjela%C5%A1nica_1984.jpg/1280px-Bjela%C5%A1nica_1984.jpg", true, "Wikimedia Commons · CC BY-SA 4.0 · Zavičajac" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc), 8, "https://upload.wikimedia.org/wikipedia/commons/1/1c/Lukomirske_udoli%2C_rozparcelovane_na_ovci_ohrady.jpg", true, "Wikimedia Commons · CC BY 3.0 · Martin Brož" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 24, 0, 0, 0, 0, DateTimeKind.Utc), 9, "https://upload.wikimedia.org/wikipedia/commons/d/d3/Vyhled_z_Orjenu_na_S_-_Trebinje_v_BaH.jpg", true, "Wikimedia Commons · CC BY 3.0 · Martin Brož" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc), 10, "https://thumb.wikimedia.org/wikipedia/commons/thumb/8/86/Vrelo_Bosne%2C_Sarajevo.jpg/1280px-Vrelo_Bosne%2C_Sarajevo.jpg", true, "Wikimedia Commons · CC BY 2.5 · Author's own photo" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc), 11, "https://thumb.wikimedia.org/wikipedia/commons/thumb/0/0d/Kravica_Waterfalls%2CBosnia_%26_Herzegovina.jpg/1280px-Kravica_Waterfalls%2CBosnia_%26_Herzegovina.jpg", true, "Wikimedia Commons · CC BY 2.0 · john mcsporran from Trossachs, Scotland" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 27, 0, 0, 0, 0, DateTimeKind.Utc), 12, "https://thumb.wikimedia.org/wikipedia/commons/thumb/e/ee/Vuk%2C_Nacionalni_park_Una.jpg/1280px-Vuk%2C_Nacionalni_park_Una.jpg", true, "Wikimedia Commons · CC BY-SA 4.0 · Nacionalni park Una" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 28, 0, 0, 0, 0, DateTimeKind.Utc), 13, "https://thumb.wikimedia.org/wikipedia/commons/thumb/4/42/Vrelo_Bune_i_tekija.jpg/1280px-Vrelo_Bune_i_tekija.jpg", true, "Wikimedia Commons · CC BY 2.0 · CJ" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 29, 0, 0, 0, 0, DateTimeKind.Utc), 14, "https://upload.wikimedia.org/wikipedia/commons/e/ea/Peru%C4%87ica.jpg", true, "Wikimedia Commons · CC BY-SA 4.0 · Minja Klarić" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 30, 0, 0, 0, 0, DateTimeKind.Utc), 15, "https://thumb.wikimedia.org/wikipedia/commons/thumb/8/82/Boracko_jezero.JPG/1280px-Boracko_jezero.JPG", true, "Wikimedia Commons · Public domain · Rochass" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Utc), 16, "https://thumb.wikimedia.org/wikipedia/commons/thumb/d/d7/Sarajevo%2C_panoramatick%C3%BD_pohled_na_Ba%C5%A1%C4%8Dar%C5%A1iji.jpg/1280px-Sarajevo%2C_panoramatick%C3%BD_pohled_na_Ba%C5%A1%C4%8Dar%C5%A1iji.jpg", true, "Wikimedia Commons · CC BY 3.0 · Aktron" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, "https://thumb.wikimedia.org/wikipedia/commons/thumb/c/ca/Iglesia_Nova_Gracanica%2C_Trebinje%2C_Bosnia_y_Herzegovina%2C_2014-04-14%2C_DD_04.jpg/1280px-Iglesia_Nova_Gracanica%2C_Trebinje%2C_Bosnia_y_Herzegovina%2C_2014-04-14%2C_DD_04.jpg", true, "Wikimedia Commons · CC BY-SA 3.0 · Diego Delso" });

            migrationBuilder.InsertData(
                table: "DestinationImages",
                columns: new[] { "Id", "AssetId", "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "OrderIndex", "Source" },
                values: new object[,]
                {
                    { 18, null, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Utc), 18, "https://thumb.wikimedia.org/wikipedia/commons/thumb/1/12/Neum_%28BiH%29_-_general_view.jpg/1280px-Neum_%28BiH%29_-_general_view.jpg", true, 0, "Wikimedia Commons · CC BY-SA 3.0 · Pudelek (Marcin Szala)" },
                    { 19, null, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Utc), 19, "https://thumb.wikimedia.org/wikipedia/commons/thumb/3/33/Mostar_Stari_Most_BW_2024-10-01_11-58-11.jpg/1280px-Mostar_Stari_Most_BW_2024-10-01_11-58-11.jpg", true, 0, "Wikimedia Commons · CC BY-SA 4.0 · Berthold Werner" },
                    { 20, null, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Utc), 20, "https://thumb.wikimedia.org/wikipedia/commons/thumb/5/5c/Zenit_Beach_in_Neum%2C_2024_2.jpg/1280px-Zenit_Beach_in_Neum%2C_2024_2.jpg", true, 0, "Wikimedia Commons · CC0 · Bdx" },
                    { 21, null, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), 21, "https://thumb.wikimedia.org/wikipedia/commons/thumb/7/76/Pla%C5%BEa_Banje_004.jpg/1280px-Pla%C5%BEa_Banje_004.jpg", true, 0, "Wikimedia Commons · CC BY-SA 4.0 · Vojtěch Dočkal" },
                    { 22, null, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Utc), 22, "https://upload.wikimedia.org/wikipedia/commons/d/dd/Bacvice.jpg", true, 0, "Wikimedia Commons · CC BY-SA 1.0 · Airin" },
                    { 23, null, new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), 23, "https://thumb.wikimedia.org/wikipedia/commons/thumb/e/e3/01_Mogren_beach.jpg/1280px-01_Mogren_beach.jpg", true, 0, "Wikimedia Commons · CC BY-SA 4.0 · BuhaM" },
                    { 24, null, new DateTime(2026, 2, 8, 0, 0, 0, 0, DateTimeKind.Utc), 24, "https://thumb.wikimedia.org/wikipedia/commons/thumb/6/65/Montenegro._Gospa_od_%C5%A0krpjela_15.jpg/1280px-Montenegro._Gospa_od_%C5%A0krpjela_15.jpg", true, 0, "Wikimedia Commons · CC BY-SA 4.0 · Oktoober" }
                });

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 3,
                column: "Keywords",
                value: "Počitelj Mostar Bosnia and Herzegovina, Medieval fortress, Neretva river, Stone architecture");

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 4,
                column: "Keywords",
                value: "Tvrđava Jajce Jajce Bosnia and Herzegovina, medieval fortress, last Bosnian royal citadel, over Pliva waterfall");

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 7,
                column: "Keywords",
                value: "Bjelašnica Sarajevo Bosnia and Herzegovina, mountain, ski resort, Olympic 1984, panoramic view");

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 8,
                column: "Keywords",
                value: "Lukomir Sarajevo Bosnia and Herzegovina, mountain village, traditional architecture, isolated settlement, highest village");

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 9,
                column: "Keywords",
                value: "Orjen Trebinje Bosnia and Herzegovina, southern Dinaric mountain, mountain massif, border with Montenegro, peaks above 1800m");

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 12,
                column: "Keywords",
                value: "Nacionalni park Una Bihać Bosnia and Herzegovina, river Una, clear water, rapids, Strbacki Buk waterfalls");

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 14,
                column: "Keywords",
                value: "Sutjeska National Park Foča Bosnia and Herzegovina, oldest national park, Perućica forest, Mount Maglić, 2386m peak");

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 15,
                column: "Keywords",
                value: "Boračko jezero Konjic Bosnia and Herzegovina, glacial lake, forested mountains, swimming, recreation");

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 17,
                column: "Keywords",
                value: "Trebinje Bosnia and Herzegovina, wine cellar, herzegovinian wine, zlatna žilavka, vranac");

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 18,
                column: "Keywords",
                value: "Neum Bosnia and Herzegovina, seafood restaurants, fresh fish, coastal cuisine, Bosnian Adriatic");

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 19,
                column: "Keywords",
                value: "Herzegovinian culinary tour Mostar Bosnia and Herzegovina, dry meat, cheese, home wine");

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 22,
                column: "Keywords",
                value: "Bačvice Split Croatia, shaped like a shell, picigin game, popular beach");

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 23,
                column: "Keywords",
                value: "Plaža Mogren Budva Crna Gora, sand pebble beach, two coves, old fort view");

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://thumb.wikimedia.org/wikipedia/commons/thumb/8/85/View_of_Sarajevo_Old_Town_at_Night..JPG/1280px-View_of_Sarajevo_Old_Town_at_Night..JPG");

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://thumb.wikimedia.org/wikipedia/commons/thumb/4/47/01_Croatia_summer_vacation_destination_-_seaside_village_Dolac_between_Primosten_and_Bilo%2C_Dalmatia.jpg/1280px-01_Croatia_summer_vacation_destination_-_seaside_village_Dolac_between_Primosten_and_Bilo%2C_Dalmatia.jpg");

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://thumb.wikimedia.org/wikipedia/commons/thumb/d/d5/Np_sutjeska_maglic.JPG/1280px-Np_sutjeska_maglic.JPG");

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/2/2b/Punjena_Paprika.JPG");

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://thumb.wikimedia.org/wikipedia/commons/thumb/d/d9/Kotor_and_Boka_kotorska_-_view_from_city_wall.jpg/1280px-Kotor_and_Boka_kotorska_-_view_from_city_wall.jpg");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 1,
                column: "Message",
                value: "Vaša recenzija destinacije Stari Most je odobrena i sada je javno vidljiva.");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 2,
                column: "Message",
                value: "Vaša recenzija destinacije Vrelo Bune (Blagaj) nije objavljena. Razlog: Neprimjeren ili netačan sadržaj recenzije.");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 3,
                column: "Message",
                value: "Vaša recenzija destinacije Plaža Neum nije objavljena. Razlog: Neprimjeren ili netačan sadržaj recenzije.");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 5,
                column: "Message",
                value: "Plan 'Sedmica u planinama BiH' je sada završen.");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 6,
                column: "Message",
                value: "Plan 'Istraživanje nacionalnih parkova' je sada otkazan.");

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "IsRead", "Message", "Title", "Type", "UserId" },
                values: new object[,]
                {
                    { 11, new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), true, "Plan 'Vikend na Uni' je sada završen.", "Status plana putovanja promijenjen", 2, 2 },
                    { 12, new DateTime(2026, 3, 8, 0, 0, 0, 0, DateTimeKind.Utc), false, "Vaša recenzija destinacije Vodopadi Kravica je odobrena i sada je javno vidljiva.", "Recenzija odobrena", 0, 2 }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "Comment", "CreatedAt", "DestinationId", "IsDeleted", "ModeratedAt", "ModeratedByUserId", "ModerationReason", "Rating", "Status", "UserId" },
                values: new object[] { 32, "Kravica nas je oduševila, voda je bila topla i čista.", new DateTime(2026, 3, 7, 0, 0, 0, 0, DateTimeKind.Utc), 11, false, new DateTime(2026, 3, 8, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 5, 1, 2 });

            migrationBuilder.InsertData(
                table: "SavedDestinations",
                columns: new[] { "Id", "DestinationId", "SavedAt", "UserId" },
                values: new object[,]
                {
                    { 21, 2, new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 22, 11, new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2 }
                });

            migrationBuilder.InsertData(
                table: "TripPlans",
                columns: new[] { "Id", "CreatedAt", "EndDate", "IsDeleted", "Name", "StartDate", "Status", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 7, new DateTime(2026, 4, 25, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), false, "Zima u Hercegovini", new DateTime(2026, 12, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 8, new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), false, "Vikend na Uni", new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Utc), 2, new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), 2 }
                });

            migrationBuilder.InsertData(
                table: "UserPreferences",
                columns: new[] { "Id", "CategoryId", "UserId" },
                values: new object[,]
                {
                    { 13, 3, 2 },
                    { 14, 5, 2 }
                });

            migrationBuilder.InsertData(
                table: "ViewHistories",
                columns: new[] { "Id", "DestinationId", "UserId", "ViewedAt" },
                values: new object[,]
                {
                    { 36, 1, 2, new DateTime(2026, 4, 29, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 37, 11, 2, new DateTime(2026, 4, 29, 6, 0, 0, 0, DateTimeKind.Utc) },
                    { 38, 13, 2, new DateTime(2026, 4, 29, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 39, 3, 2, new DateTime(2026, 4, 29, 18, 0, 0, 0, DateTimeKind.Utc) },
                    { 40, 12, 2, new DateTime(2026, 4, 30, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "TripPlanItems",
                columns: new[] { "Id", "DayNumber", "DestinationId", "Notes", "OrderIndex", "TripPlanId" },
                values: new object[,]
                {
                    { 19, 1, 1, "Šetnja Starim mostom u zoru.", 1, 7 },
                    { 20, 1, 3, null, 2, 7 },
                    { 21, 2, 13, "Ručak uz Vrelo Bune.", 1, 7 },
                    { 22, 1, 12, "Rafting na Uni.", 1, 8 },
                    { 23, 2, 15, null, 1, 8 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AuditLogs",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AuditLogs",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AuditLogs",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "SavedDestinations",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "SavedDestinations",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "TripPlanItems",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "TripPlanItems",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "TripPlanItems",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "TripPlanItems",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "TripPlanItems",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "UserPreferences",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "UserPreferences",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ViewHistories",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "ViewHistories",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "ViewHistories",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "ViewHistories",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "ViewHistories",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "TripPlans",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TripPlans",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.UpdateData(
                table: "Collections",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Mobile test kolekcija");

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { "https://picsum.photos/seed/travelbay-1/800/600", false, "Seed data" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { "https://picsum.photos/seed/travelbay-2/800/600", false, "Seed data" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { "https://picsum.photos/seed/travelbay-3/800/600", false, "Seed data" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), 5, "https://picsum.photos/seed/travelbay-5/800/600", false, "Seed data" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), 6, "https://picsum.photos/seed/travelbay-6/800/600", false, "Seed data" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 22, 0, 0, 0, 0, DateTimeKind.Utc), 7, "https://picsum.photos/seed/travelbay-7/800/600", false, "Seed data" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc), 10, "https://picsum.photos/seed/travelbay-10/800/600", false, "Seed data" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc), 11, "https://picsum.photos/seed/travelbay-11/800/600", false, "Seed data" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 27, 0, 0, 0, 0, DateTimeKind.Utc), 12, "https://picsum.photos/seed/travelbay-12/800/600", false, "Seed data" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 28, 0, 0, 0, 0, DateTimeKind.Utc), 13, "https://picsum.photos/seed/travelbay-13/800/600", false, "Seed data" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 30, 0, 0, 0, 0, DateTimeKind.Utc), 15, "https://picsum.photos/seed/travelbay-15/800/600", false, "Seed data" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Utc), 16, "https://picsum.photos/seed/travelbay-16/800/600", false, "Seed data" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Utc), 19, "https://picsum.photos/seed/travelbay-19/800/600", false, "Seed data" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Utc), 20, "https://picsum.photos/seed/travelbay-20/800/600", false, "Seed data" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), 21, "https://picsum.photos/seed/travelbay-21/800/600", false, "Seed data" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Utc), 22, "https://picsum.photos/seed/travelbay-22/800/600", false, "Seed data" });

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CreatedAt", "DestinationId", "ImageUrl", "IsAiGenerated", "Source" },
                values: new object[] { new DateTime(2026, 2, 8, 0, 0, 0, 0, DateTimeKind.Utc), 24, "https://picsum.photos/seed/travelbay-24/800/600", false, "Seed data" });

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 3,
                column: "Keywords",
                value: null);

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 4,
                column: "Keywords",
                value: null);

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 7,
                column: "Keywords",
                value: null);

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 8,
                column: "Keywords",
                value: null);

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 9,
                column: "Keywords",
                value: null);

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 12,
                column: "Keywords",
                value: null);

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 14,
                column: "Keywords",
                value: null);

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 15,
                column: "Keywords",
                value: null);

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 17,
                column: "Keywords",
                value: null);

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 18,
                column: "Keywords",
                value: null);

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 19,
                column: "Keywords",
                value: null);

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 22,
                column: "Keywords",
                value: null);

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 23,
                column: "Keywords",
                value: null);

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://picsum.photos/seed/travelbay-news-1/800/400");

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://picsum.photos/seed/travelbay-news-2/800/400");

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://picsum.photos/seed/travelbay-news-3/800/400");

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "https://picsum.photos/seed/travelbay-news-4/800/400");

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://picsum.photos/seed/travelbay-news-5/800/400");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 1,
                column: "Message",
                value: "Vaša recenzija za Stari Most je odobrena.");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 2,
                column: "Message",
                value: "Vaša recenzija za Vrelo Bune nije odobrena.");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 3,
                column: "Message",
                value: "Vaša recenzija za Plažu Neum nije odobrena.");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 5,
                column: "Message",
                value: "Plan 'Sedmica u planinama BiH' je završen.");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 6,
                column: "Message",
                value: "Plan 'Istraživanje nacionalnih parkova' je otkazan.");
        }
    }
}
