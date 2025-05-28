using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Repository.Models;

public partial class PizzaShopContext : DbContext
{
    public PizzaShopContext()
    {
    }

    public PizzaShopContext(DbContextOptions<PizzaShopContext> options)
        : base(options)
    {
    }
    public DbSet<KOTFlatData> KOTFlatData { get; set; } = null!;
    public DbSet<WaitingTokenViewModelRaw> WaitingTokenViewModelRaw { get; set; } = null!;


    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Customerreview> Customerreviews { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Itemmodifiergroupmap> Itemmodifiergroupmaps { get; set; }

    public virtual DbSet<Modifier> Modifiers { get; set; }

    public virtual DbSet<ModifierGroupModifierMapping> ModifierGroupModifierMappings { get; set; }

    public virtual DbSet<Modifiergroup> Modifiergroups { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Ordereditem> Ordereditems { get; set; }

    public virtual DbSet<Ordereditemmodifer> Ordereditemmodifers { get; set; }

    public virtual DbSet<Ordertable> Ordertables { get; set; }

    public virtual DbSet<Ordertaxmapping> Ordertaxmappings { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Permissionmodule> Permissionmodules { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<Taxis> Taxes { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userslogin> Userslogins { get; set; }

    public virtual DbSet<Waitingtablemapping> Waitingtablemappings { get; set; }

    public virtual DbSet<Waitingtoken> Waitingtokens { get; set; }

#pragma warning disable CS1030
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=PizzaShop;Username=postgres; password=Tatva@123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("itemtype", new[] { "Veg", "Non-Veg", "Vegan" })
            .HasPostgresEnum("orderstatus", new[] { "InProgress", "Pending", "Served", "Completed", "Cancelled", "On Hold", "Failed" })
            .HasPostgresEnum("statustype", new[] { "Active", "Inactive" })
            .HasPostgresEnum("tablestatus", new[] { "Available", "Occupied", "Reserved" });

        modelBuilder.Entity<KOTFlatData>().HasNoKey();
        modelBuilder.Entity<WaitingTokenViewModel>().HasNoKey();
        modelBuilder.Entity<WaitingTokenViewModelRaw>().HasNoKey();
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Categoryid).HasName("category_pkey");

            entity.ToTable("category");

            entity.HasIndex(e => e.Categoryname, "category_categoryname_key").IsUnique();

            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Categoryname)
                .HasMaxLength(30)
                .HasColumnName("categoryname");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("isdeleted");
            entity.Property(e => e.Modifiedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedat");
            entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");
            entity.Property(e => e.Sortorder).HasColumnName("sortorder");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Cityid).HasName("city_pkey");

            entity.ToTable("city");

            entity.Property(e => e.Cityid).HasColumnName("cityid");
            entity.Property(e => e.Cityname)
                .HasMaxLength(50)
                .HasColumnName("cityname");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby)
                .HasDefaultValueSql("1")
                .HasColumnName("createdby");
            entity.Property(e => e.Modifiedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedat");
            entity.Property(e => e.Modifiedby)
                .HasDefaultValueSql("1")
                .HasColumnName("modifiedby");
            entity.Property(e => e.Stateid).HasColumnName("stateid");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Countryid).HasName("countries_pkey");

            entity.ToTable("countries");

            entity.HasIndex(e => e.Shortname, "countries_shortname_key").IsUnique();

            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Countryname)
                .HasMaxLength(150)
                .HasColumnName("countryname");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby)
                .HasDefaultValueSql("1")
                .HasColumnName("createdby");
            entity.Property(e => e.Modifiedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedat");
            entity.Property(e => e.Modifiedby)
                .HasDefaultValueSql("1")
                .HasColumnName("modifiedby");
            entity.Property(e => e.Phonecode).HasColumnName("phonecode");
            entity.Property(e => e.Shortname)
                .HasMaxLength(3)
                .HasColumnName("shortname");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Customerid).HasName("customers_pkey");

            entity.ToTable("customers");

            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Customername)
                .HasMaxLength(30)
                .HasColumnName("customername");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Modifiedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("modifiedat");
            entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");
            entity.Property(e => e.Ordertype)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Dining'::character varying")
                .HasColumnName("ordertype");
            entity.Property(e => e.Phoneno)
                .HasMaxLength(15)
                .HasColumnName("phoneno");
            entity.Property(e => e.Totalorder)
                .HasDefaultValueSql("0")
                .HasColumnName("totalorder");
            entity.Property(e => e.Visitcount)
                .HasDefaultValueSql("0")
                .HasColumnName("visitcount");
        });

        modelBuilder.Entity<Customerreview>(entity =>
        {
            entity.HasKey(e => e.Reviewid).HasName("customerreviews_pkey");

            entity.ToTable("customerreviews");

            entity.Property(e => e.Reviewid).HasColumnName("reviewid");
            entity.Property(e => e.Ambiencerating)
                .HasPrecision(1)
                .HasColumnName("ambiencerating");
            entity.Property(e => e.Avgrating).HasColumnName("avgrating");
            entity.Property(e => e.Comments).HasColumnName("comments");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Foodrating)
                .HasPrecision(1)
                .HasColumnName("foodrating");
            entity.Property(e => e.Modifiedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedat");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Servicerating)
                .HasPrecision(1)
                .HasColumnName("servicerating");

            entity.HasOne(d => d.Order).WithMany(p => p.Customerreviews)
                .HasForeignKey(d => d.Orderid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customerreviews_orderid_fkey");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Invoiceid).HasName("invoice_pkey");

            entity.ToTable("invoice");

            entity.HasIndex(e => e.Invoicenumber, "invoice_invoicenumber_key").IsUnique();

            entity.Property(e => e.Invoiceid).HasColumnName("invoiceid");
            entity.Property(e => e.Invoicedate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("invoicedate");
            entity.Property(e => e.Invoicenumber)
                .HasMaxLength(50)
                .HasColumnName("invoicenumber");
            entity.Property(e => e.Orderid).HasColumnName("orderid");

            entity.HasOne(d => d.Order).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.Orderid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("invoice_orderid_fkey");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Itemid).HasName("items_pkey");

            entity.ToTable("items");

            entity.HasIndex(e => e.Itemname, "items_itemname_key").IsUnique();

            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Isavailable)
                .HasDefaultValueSql("false")
                .HasColumnName("isavailable");
            entity.Property(e => e.Isdefaulttax)
                .HasDefaultValueSql("false")
                .HasColumnName("isdefaulttax");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("isdeleted");
            entity.Property(e => e.Isfavourite)
                .HasDefaultValueSql("false")
                .HasColumnName("isfavourite");
            entity.Property(e => e.Itemimg).HasColumnName("itemimg");
            entity.Property(e => e.Itemname).HasColumnName("itemname");
            entity.Property(e => e.Itemtype)
                .HasMaxLength(10)
                .HasDefaultValueSql("'Veg'::character varying")
                .HasColumnName("itemtype");
            entity.Property(e => e.Modifiedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedat");
            entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("1")
                .HasColumnName("quantity");
            entity.Property(e => e.Rate)
                .HasPrecision(10, 2)
                .HasColumnName("rate");
            entity.Property(e => e.Shortcode)
                .HasMaxLength(30)
                .HasColumnName("shortcode");
            entity.Property(e => e.Taxpercentage)
                .HasPrecision(5, 2)
                .HasColumnName("taxpercentage");
            entity.Property(e => e.Unitid).HasColumnName("unitid");

            entity.HasOne(d => d.Category).WithMany(p => p.Items)
                .HasForeignKey(d => d.Categoryid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("items_categoryid_fkey");

            entity.HasOne(d => d.Unit).WithMany(p => p.Items)
                .HasForeignKey(d => d.Unitid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("items_unitid_fkey");
        });

        modelBuilder.Entity<Itemmodifiergroupmap>(entity =>
        {
            entity.HasKey(e => e.Itemmodifiergroupid).HasName("itemmodifiergroupmap_pkey");

            entity.ToTable("itemmodifiergroupmap");

            entity.Property(e => e.Itemmodifiergroupid).HasColumnName("itemmodifiergroupid");
            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Maxselectionallowed).HasColumnName("maxselectionallowed");
            entity.Property(e => e.Minselectionrequired)
                .HasDefaultValueSql("0")
                .HasColumnName("minselectionrequired");
            entity.Property(e => e.Modifiergroupid).HasColumnName("modifiergroupid");

            entity.HasOne(d => d.Item).WithMany(p => p.Itemmodifiergroupmaps)
                .HasForeignKey(d => d.Itemid)
                .HasConstraintName("fk_itemmodifiergroupmap_items");

            entity.HasOne(d => d.Modifiergroup).WithMany(p => p.Itemmodifiergroupmaps)
                .HasForeignKey(d => d.Modifiergroupid)
                .HasConstraintName("fk_itemmodifiergroupmap_modifiergroup");
        });

        modelBuilder.Entity<Modifier>(entity =>
        {
            entity.HasKey(e => e.Modifierid).HasName("modifiers_pkey");

            entity.ToTable("modifiers");

            entity.HasIndex(e => e.Modifiername, "modifiers_modifiername_key").IsUnique();

            entity.Property(e => e.Modifierid).HasColumnName("modifierid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("isdeleted");
            entity.Property(e => e.Modifiedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedat");
            entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");
            entity.Property(e => e.Modifiername).HasColumnName("modifiername");
            entity.Property(e => e.Modifiertype).HasColumnName("modifiertype");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Rate)
                .HasPrecision(10, 2)
                .HasColumnName("rate");
            entity.Property(e => e.Unitid).HasColumnName("unitid");

            entity.HasOne(d => d.Unit).WithMany(p => p.Modifiers)
                .HasForeignKey(d => d.Unitid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifiers_unitid_fkey");
        });

        modelBuilder.Entity<ModifierGroupModifierMapping>(entity =>
        {
            entity.HasKey(e => e.ModifierGroupModifierMappingId).HasName("ModifierGroupModifierMapping_pkey");

            entity.ToTable("ModifierGroupModifierMapping");

            entity.HasOne(d => d.ModifierGroup).WithMany(p => p.ModifierGroupModifierMappings)
                .HasForeignKey(d => d.ModifierGroupId)
                .HasConstraintName("ModifierGroupModifierMapping_ModifierGroupId_fkey");

            entity.HasOne(d => d.Modifier).WithMany(p => p.ModifierGroupModifierMappings)
                .HasForeignKey(d => d.ModifierId)
                .HasConstraintName("ModifierGroupModifierMapping_ModifierId_fkey");
        });

        modelBuilder.Entity<Modifiergroup>(entity =>
        {
            entity.HasKey(e => e.Modifiergroupid).HasName("modifiergroup_pkey");

            entity.ToTable("modifiergroup");

            entity.HasIndex(e => e.Modifiergroupname, "modifiergroup_modifiergroupname_key").IsUnique();

            entity.Property(e => e.Modifiergroupid).HasColumnName("modifiergroupid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("isdeleted");
            entity.Property(e => e.Modifiedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedat");
            entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");
            entity.Property(e => e.Modifiergroupname)
                .HasMaxLength(30)
                .HasColumnName("modifiergroupname");
            entity.Property(e => e.Sortorder).HasColumnName("sortorder");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Orderid).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("timezone('Asia/Kolkata'::text, CURRENT_TIMESTAMP)")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Discount)
                .HasPrecision(3)
                .HasColumnName("discount");
            entity.Property(e => e.InvoiceNumber)
                .HasMaxLength(50)
                .HasColumnName("invoice_number");
            entity.Property(e => e.Modifiedat)
                .HasDefaultValueSql("timezone('Asia/Kolkata'::text, CURRENT_TIMESTAMP)")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedat");
            entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");
            entity.Property(e => e.Noofperson).HasColumnName("noofperson");
            entity.Property(e => e.Orderdate)
                .HasDefaultValueSql("timezone('Asia/Kolkata'::text, CURRENT_TIMESTAMP)")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("orderdate");
            entity.Property(e => e.Ordertype)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Dining'::character varying")
                .HasColumnName("ordertype");
            entity.Property(e => e.Orderwisecomment).HasColumnName("orderwisecomment");
            entity.Property(e => e.Paymentmode)
                .HasMaxLength(20)
                .HasColumnName("paymentmode");
            entity.Property(e => e.Rating)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("rating");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Subamount)
                .HasPrecision(10, 2)
                .HasColumnName("subamount");
            entity.Property(e => e.Totalamount)
                .HasPrecision(10, 2)
                .HasColumnName("totalamount");
            entity.Property(e => e.Totaltax)
                .HasPrecision(10, 2)
                .HasColumnName("totaltax");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_customerid_fkey");
        });

        modelBuilder.Entity<Ordereditem>(entity =>
        {
            entity.HasKey(e => e.Ordereditemid).HasName("ordereditems_pkey");

            entity.ToTable("ordereditems");

            entity.Property(e => e.Ordereditemid).HasColumnName("ordereditemid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Itemwisecomment).HasColumnName("itemwisecomment");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("1")
                .HasColumnName("quantity");
            entity.Property(e => e.Readyquantity).HasColumnName("readyquantity");
            entity.Property(e => e.Servedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("servedat");

            entity.HasOne(d => d.Item).WithMany(p => p.Ordereditems)
                .HasForeignKey(d => d.Itemid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordereditems_itemid_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Ordereditems)
                .HasForeignKey(d => d.Orderid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordereditems_orderid_fkey");
        });

        modelBuilder.Entity<Ordereditemmodifer>(entity =>
        {
            entity.HasKey(e => e.Modifieditemid).HasName("ordereditemmodifer_pkey");

            entity.ToTable("ordereditemmodifer");

            entity.Property(e => e.Modifieditemid).HasColumnName("modifieditemid");
            entity.Property(e => e.Itemmodifierid)
                .HasDefaultValueSql("1")
                .HasColumnName("itemmodifierid");
            entity.Property(e => e.Ordereditemid).HasColumnName("ordereditemid");
            entity.Property(e => e.Orderid)
                .HasDefaultValueSql("1")
                .HasColumnName("orderid");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("1")
                .HasColumnName("quantity");

            entity.HasOne(d => d.Itemmodifier).WithMany(p => p.Ordereditemmodifers)
                .HasForeignKey(d => d.Itemmodifierid)
                .HasConstraintName("ordereditemmodifer_itemmodifierid_fkey");

            entity.HasOne(d => d.Ordereditem).WithMany(p => p.Ordereditemmodifers)
                .HasForeignKey(d => d.Ordereditemid)
                .HasConstraintName("ordereditemmodifer_ordereditemid_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Ordereditemmodifers)
                .HasForeignKey(d => d.Orderid)
                .HasConstraintName("ordereditemmodifer_orderid_fkey");
        });

        modelBuilder.Entity<Ordertable>(entity =>
        {
            entity.HasKey(e => e.Ordertableid).HasName("ordertable_pkey");

            entity.ToTable("ordertable");

            entity.Property(e => e.Ordertableid).HasColumnName("ordertableid");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Tableid).HasColumnName("tableid");

            entity.HasOne(d => d.Order).WithMany(p => p.Ordertables)
                .HasForeignKey(d => d.Orderid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordertable_orderid_fkey");

            entity.HasOne(d => d.Table).WithMany(p => p.Ordertables)
                .HasForeignKey(d => d.Tableid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordertable_tableid_fkey");
        });

        modelBuilder.Entity<Ordertaxmapping>(entity =>
        {
            entity.HasKey(e => e.Ordertaxid).HasName("ordertaxmapping_pkey");

            entity.ToTable("ordertaxmapping");

            entity.Property(e => e.Ordertaxid).HasColumnName("ordertaxid");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Taxid).HasColumnName("taxid");
            entity.Property(e => e.Taxvalue).HasColumnName("taxvalue");

            entity.HasOne(d => d.Order).WithMany(p => p.Ordertaxmappings)
                .HasForeignKey(d => d.Orderid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordertaxmapping_orderid_fkey");

            entity.HasOne(d => d.Tax).WithMany(p => p.Ordertaxmappings)
                .HasForeignKey(d => d.Taxid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordertaxmapping_taxid_fkey");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PasswordResetTokens_pkey");

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResetTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Permissionid).HasName("permission_pkey");

            entity.ToTable("permission");

            entity.Property(e => e.Permissionid).HasColumnName("permissionid");
            entity.Property(e => e.Canaddedit)
                .HasDefaultValueSql("false")
                .HasColumnName("canaddedit");
            entity.Property(e => e.Candelete)
                .HasDefaultValueSql("false")
                .HasColumnName("candelete");
            entity.Property(e => e.Canview)
                .HasDefaultValueSql("false")
                .HasColumnName("canview");
            entity.Property(e => e.Moduleid).HasColumnName("moduleid");
            entity.Property(e => e.Roleid).HasColumnName("roleid");

            entity.HasOne(d => d.Module).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.Moduleid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("permission_moduleid_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("permission_roleid_fkey");
        });

        modelBuilder.Entity<Permissionmodule>(entity =>
        {
            entity.HasKey(e => e.Moduleid).HasName("permissionmodule_pkey");

            entity.ToTable("permissionmodule");

            entity.HasIndex(e => e.Modulename, "permissionmodule_modulename_key").IsUnique();

            entity.Property(e => e.Moduleid).HasColumnName("moduleid");
            entity.Property(e => e.Modulename)
                .HasMaxLength(30)
                .HasColumnName("modulename");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Rolename, "roles_rolename_key").IsUnique();

            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Rolename)
                .HasMaxLength(30)
                .HasColumnName("rolename");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.Sectionid).HasName("sections_pkey");

            entity.ToTable("sections");

            entity.HasIndex(e => e.Sectionname, "sections_sectionname_key").IsUnique();

            entity.Property(e => e.Sectionid).HasColumnName("sectionid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("isdeleted");
            entity.Property(e => e.Modifiedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedat");
            entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");
            entity.Property(e => e.Sectionname)
                .HasMaxLength(30)
                .HasColumnName("sectionname");
            entity.Property(e => e.Sectionorder)
                .ValueGeneratedOnAdd()
                .HasColumnName("sectionorder");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Stateid).HasName("state_pkey");

            entity.ToTable("state");

            entity.Property(e => e.Stateid).HasColumnName("stateid");
            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby)
                .HasDefaultValueSql("1")
                .HasColumnName("createdby");
            entity.Property(e => e.Modifiedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedat");
            entity.Property(e => e.Modifiedby)
                .HasDefaultValueSql("1")
                .HasColumnName("modifiedby");
            entity.Property(e => e.Statename)
                .HasMaxLength(50)
                .HasColumnName("statename");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.Tableid).HasName("tables_pkey");

            entity.ToTable("tables");

            entity.Property(e => e.Tableid).HasColumnName("tableid");
            entity.Property(e => e.Capacity)
                .HasPrecision(2)
                .HasColumnName("capacity");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("isdeleted");
            entity.Property(e => e.Modifiedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedat");
            entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");
            entity.Property(e => e.Sectionid).HasColumnName("sectionid");
            entity.Property(e => e.Tablename)
                .HasMaxLength(20)
                .HasColumnName("tablename");
            entity.Property(e => e.Tablestatus)
                .HasDefaultValueSql("1")
                .HasColumnName("tablestatus");

            entity.HasOne(d => d.Section).WithMany(p => p.Tables)
                .HasForeignKey(d => d.Sectionid)
                .HasConstraintName("tables_sectionid_fkey");
        });

        modelBuilder.Entity<Taxis>(entity =>
        {
            entity.HasKey(e => e.Taxid).HasName("taxes_pkey");

            entity.ToTable("taxes");

            entity.Property(e => e.Taxid).HasColumnName("taxid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Isdefault).HasColumnName("isdefault");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("isdeleted");
            entity.Property(e => e.Isenabled).HasColumnName("isenabled");
            entity.Property(e => e.Isinclusive)
                .HasDefaultValueSql("false")
                .HasColumnName("isinclusive");
            entity.Property(e => e.Modifiedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedat");
            entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");
            entity.Property(e => e.Taxname)
                .HasMaxLength(10)
                .HasColumnName("taxname");
            entity.Property(e => e.Taxtype)
                .HasMaxLength(20)
                .HasColumnName("taxtype");
            entity.Property(e => e.Taxvalue).HasColumnName("taxvalue");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Unitid).HasName("units_pkey");

            entity.ToTable("units");

            entity.Property(e => e.Unitid).HasColumnName("unitid");
            entity.Property(e => e.Shortname)
                .HasMaxLength(20)
                .HasColumnName("shortname");
            entity.Property(e => e.Unitname)
                .HasMaxLength(50)
                .HasColumnName("unitname");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Phone, "users_phone_key").IsUnique();

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Cityid).HasColumnName("cityid");
            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Email).HasColumnType("character varying");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("isdeleted");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Modifiedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedat");
            entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");
            entity.Property(e => e.Password).HasColumnType("character varying");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.Profileimg).HasColumnName("profileimg");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Stateid).HasColumnName("stateid");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("1")
                .HasColumnName("status");
            entity.Property(e => e.Username).HasColumnType("character varying");
            entity.Property(e => e.Zipcode)
                .HasMaxLength(10)
                .HasColumnName("zipcode");
        });

        modelBuilder.Entity<Userslogin>(entity =>
        {
            entity.HasKey(e => e.Userloginid).HasName("userslogin_pkey");

            entity.ToTable("userslogin");

            entity.HasIndex(e => e.Email, "userslogin_email_key").IsUnique();

            entity.HasIndex(e => e.Refreshtoken, "userslogin_refreshtoken_key").IsUnique();

            entity.HasIndex(e => e.Resettoken, "userslogin_resettoken_key").IsUnique();

            entity.HasIndex(e => e.Username, "userslogin_username_key").IsUnique();

            entity.Property(e => e.Userloginid).HasColumnName("userloginid");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Isfirstlogin).HasColumnName("isfirstlogin");
            entity.Property(e => e.Passwordhash).HasColumnName("passwordhash");
            entity.Property(e => e.Refreshtoken).HasColumnName("refreshtoken");
            entity.Property(e => e.Resettoken).HasColumnName("resettoken");
            entity.Property(e => e.Resettokenexpiration)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("resettokenexpiration");
            entity.Property(e => e.Resettokenused)
                .HasDefaultValueSql("false")
                .HasColumnName("resettokenused");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Userslogins)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userslogin_roleid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Userslogins)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("userslogin_userid_fkey");
        });

        modelBuilder.Entity<Waitingtablemapping>(entity =>
        {
            entity.HasKey(e => e.Waitingtableid).HasName("waitingtablemapping_pkey");

            entity.ToTable("waitingtablemapping");

            entity.Property(e => e.Waitingtableid).HasColumnName("waitingtableid");
            entity.Property(e => e.Tableid).HasColumnName("tableid");
            entity.Property(e => e.Waitingtokenid).HasColumnName("waitingtokenid");

            entity.HasOne(d => d.Table).WithMany(p => p.Waitingtablemappings)
                .HasForeignKey(d => d.Tableid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("waitingtablemapping_tableid_fkey");

            entity.HasOne(d => d.Waitingtoken).WithMany(p => p.Waitingtablemappings)
                .HasForeignKey(d => d.Waitingtokenid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("waitingtablemapping_waitingtokenid_fkey");
        });

        modelBuilder.Entity<Waitingtoken>(entity =>
        {
            entity.HasKey(e => e.Waitingtokenid).HasName("waitingtoken_pkey");

            entity.ToTable("waitingtoken");

            entity.Property(e => e.Waitingtokenid).HasColumnName("waitingtokenid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Isassigned)
                .HasDefaultValueSql("false")
                .HasColumnName("isassigned");
            entity.Property(e => e.Modifiedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("modifiedat");
            entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");
            entity.Property(e => e.Noofpeople).HasColumnName("noofpeople");
            entity.Property(e => e.Sectionid).HasColumnName("sectionid");

            entity.HasOne(d => d.Customer).WithMany(p => p.Waitingtokens)
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("waitingtoken_customerid_fkey");

            entity.HasOne(d => d.Section).WithMany(p => p.Waitingtokens)
                .HasForeignKey(d => d.Sectionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("waitingtoken_sectionid_fkey");
        });
        modelBuilder.HasSequence("city_cityid_seq");
        modelBuilder.HasSequence("country_countryid_seq");
        modelBuilder.HasSequence("state_stateid_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
