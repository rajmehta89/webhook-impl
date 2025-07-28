using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsappWebHook.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaFieldsToWhatsAppMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MediaContentType",
                table: "WhatsAppMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MediaSid",
                table: "WhatsAppMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MediaUrl",
                table: "WhatsAppMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaContentType",
                table: "WhatsAppMessages");

            migrationBuilder.DropColumn(
                name: "MediaSid",
                table: "WhatsAppMessages");

            migrationBuilder.DropColumn(
                name: "MediaUrl",
                table: "WhatsAppMessages");
        }
    }
}
