using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_project.Migrations
{
    public partial class account_detail_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizeColor_Color_ColorId",
                table: "ProductSizeColor");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizeColor_Products_ProductId",
                table: "ProductSizeColor");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizeColor_Size_SizeId",
                table: "ProductSizeColor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Size",
                table: "Size");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductSizeColor",
                table: "ProductSizeColor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Color",
                table: "Color");

            migrationBuilder.RenameTable(
                name: "Size",
                newName: "Sizes");

            migrationBuilder.RenameTable(
                name: "ProductSizeColor",
                newName: "ProductSizeColors");

            migrationBuilder.RenameTable(
                name: "Color",
                newName: "Colors");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSizeColor_SizeId",
                table: "ProductSizeColors",
                newName: "IX_ProductSizeColors_SizeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSizeColor_ProductId",
                table: "ProductSizeColors",
                newName: "IX_ProductSizeColors_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSizeColor_ColorId",
                table: "ProductSizeColors",
                newName: "IX_ProductSizeColors_ColorId");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "Colors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sizes",
                table: "Sizes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductSizeColors",
                table: "ProductSizeColors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Colors",
                table: "Colors",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AccountDetails",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountDetails", x => x.UserId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizeColors_Colors_ColorId",
                table: "ProductSizeColors",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizeColors_Products_ProductId",
                table: "ProductSizeColors",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizeColors_Sizes_SizeId",
                table: "ProductSizeColors",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizeColors_Colors_ColorId",
                table: "ProductSizeColors");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizeColors_Products_ProductId",
                table: "ProductSizeColors");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizeColors_Sizes_SizeId",
                table: "ProductSizeColors");

            migrationBuilder.DropTable(
                name: "AccountDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sizes",
                table: "Sizes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductSizeColors",
                table: "ProductSizeColors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Colors",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "Colors");

            migrationBuilder.RenameTable(
                name: "Sizes",
                newName: "Size");

            migrationBuilder.RenameTable(
                name: "ProductSizeColors",
                newName: "ProductSizeColor");

            migrationBuilder.RenameTable(
                name: "Colors",
                newName: "Color");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSizeColors_SizeId",
                table: "ProductSizeColor",
                newName: "IX_ProductSizeColor_SizeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSizeColors_ProductId",
                table: "ProductSizeColor",
                newName: "IX_ProductSizeColor_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSizeColors_ColorId",
                table: "ProductSizeColor",
                newName: "IX_ProductSizeColor_ColorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Size",
                table: "Size",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductSizeColor",
                table: "ProductSizeColor",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Color",
                table: "Color",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizeColor_Color_ColorId",
                table: "ProductSizeColor",
                column: "ColorId",
                principalTable: "Color",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizeColor_Products_ProductId",
                table: "ProductSizeColor",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizeColor_Size_SizeId",
                table: "ProductSizeColor",
                column: "SizeId",
                principalTable: "Size",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
