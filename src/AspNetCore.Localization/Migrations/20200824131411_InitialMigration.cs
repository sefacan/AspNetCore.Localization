using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCore.Localization.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    Culture = table.Column<string>(maxLength: 10, nullable: true),
                    TwoLetterIsoCode = table.Column<string>(maxLength: 2, nullable: true),
                    ThreeLetterIsoCode = table.Column<string>(maxLength: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LanguageResources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LanguageId = table.Column<int>(nullable: false),
                    Key = table.Column<string>(maxLength: 255, nullable: true),
                    Value = table.Column<string>(maxLength: 3000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LanguageResources_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Culture", "Name", "ThreeLetterIsoCode", "TwoLetterIsoCode" },
                values: new object[] { 1, "en-US", "English", "eng", "en" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Culture", "Name", "ThreeLetterIsoCode", "TwoLetterIsoCode" },
                values: new object[] { 2, "fr-FR", "French", "fre", "fr" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Culture", "Name", "ThreeLetterIsoCode", "TwoLetterIsoCode" },
                values: new object[] { 3, "de-DE", "German", "ger", "de" });

            migrationBuilder.InsertData(
                table: "LanguageResources",
                columns: new[] { "Id", "Key", "LanguageId", "Value" },
                values: new object[] { 1, "menu.homepage", 1, "Home Page" });

            migrationBuilder.InsertData(
                table: "LanguageResources",
                columns: new[] { "Id", "Key", "LanguageId", "Value" },
                values: new object[] { 2, "menu.homepage", 2, "Page D'accueil" });

            migrationBuilder.InsertData(
                table: "LanguageResources",
                columns: new[] { "Id", "Key", "LanguageId", "Value" },
                values: new object[] { 3, "menu.homepage", 3, "Startseite" });

            migrationBuilder.CreateIndex(
                name: "IX_LanguageResources_LanguageId",
                table: "LanguageResources",
                column: "LanguageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LanguageResources");

            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}
