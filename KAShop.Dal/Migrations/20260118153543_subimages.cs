using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KAShop.Dal.Migrations
{
    /// <inheritdoc />
    public partial class subimages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "productImages",
                newName: "ImageName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageName",
                table: "productImages",
                newName: "Name");
        }
    }
}
