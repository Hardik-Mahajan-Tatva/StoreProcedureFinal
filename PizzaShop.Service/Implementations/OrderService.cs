using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pizzashop.Repository.Interfaces;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderedItemRepository _orderItemRepository;
        private readonly IOrderItemModifierRepository _orderItemModifierRepository;
        private readonly ITaxesFeesRepository _taxfees;
        private readonly IItemRepository _itemRepository;
        private readonly IOrderTaxRepository _orderTaxRepository;
        private readonly IOrderTaxService _orderTaxService;
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            ILogger<OrderService> logger,
            ITaxesFeesRepository taxfees,
            IOrderedItemRepository orderItemRepository,
            IOrderItemModifierRepository orderItemModifierRepository,
            IItemRepository itemRepository,
            IOrderTaxRepository orderTaxRepository,
            ICustomerRepository customerRepository,
            IOrderTaxService orderTaxService
        )
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _orderItemModifierRepository = orderItemModifierRepository;
            _taxfees = taxfees;
            _itemRepository = itemRepository;
            _orderTaxRepository = orderTaxRepository;
            _orderTaxService = orderTaxService;
            _logger = logger;
        }
        #region  GetOrderAsynPaginated
        public async Task<PaginatedList<OrderViewModel>> GetOrdersAsync(
            string search,
            string status,
            DateTime? startDate,
            DateTime? endDate,
            int page,
            int pageSize,
            string sortColumn,
            string sortDirection
        )
        {
            var orders = await _orderRepository.GetOrdersAsync(
                search,
                status,
                startDate,
                endDate,
                page,
                pageSize,
                sortColumn,
                sortDirection
            );

            var orderViewModels = orders
                .Select(
                    o =>
                        new OrderViewModel
                        {
                            OrderId = o.Orderid,
                            CustomerName = o.Customer?.Customername ?? "Unknown",
                            OrderDate = o.Orderdate ?? DateTime.Now,
                            TotalAmount = o.Totalamount,
                            Rating = o.Rating ?? 0m,
                            PaymentMode = o.Paymentmode ?? "NA",
                            Status = (OrderStatus)o.Status
                        }
                )
                .ToList();

            return new PaginatedList<OrderViewModel>(
                orderViewModels,
                orders.TotalItems,
                page,
                pageSize
            );
        }
        #endregion

        #region  GetFilteredCustomers
        public IEnumerable<Order> GetFilteredCustomers(
            string searchText,
            DateTime? startDate,
            DateTime? endDate,
            int? orderStatus,
            string sortColumn,
            string sortOrder
        )
        {
            var query = _orderRepository.GetAll();

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(
                    o =>
                        EF.Functions.ILike(o.Customer.Customername, $"%{searchText}%")
                        || EF.Functions.ILike(o.Orderid.ToString(), $"%{searchText}%")
                );
            }

            if (startDate.HasValue)
            {
                query = query.Where(o => o.Orderdate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(o => o.Orderdate <= endDate.Value);
            }

            if (orderStatus.HasValue)
            {
                query = query.Where(o => o.Status == orderStatus.Value);
            }
            switch (sortColumn)
            {
                case "CustomerName":
                    query =
                        sortOrder == "asc"
                            ? query.OrderBy(o => o.Customer.Customername)
                            : query.OrderByDescending(o => o.Customer.Customername);
                    break;
                case "OrderDate":
                    query =
                        sortOrder == "asc"
                            ? query.OrderBy(o => o.Orderdate)
                            : query.OrderByDescending(o => o.Orderdate);
                    break;
                case "TotalAmount":
                    query =
                        sortOrder == "asc"
                            ? query.OrderBy(o => o.Totalamount)
                            : query.OrderByDescending(o => o.Totalamount);
                    break;
                default:
                    query =
                        sortOrder == "asc"
                            ? query.OrderBy(o => o.Orderid)
                            : query.OrderByDescending(o => o.Orderid);
                    break;
            }

            return query.ToList();
        }
        #endregion

        #region  GetOrderInvoiceAsync
        public async Task<OrderInvoiceViewModel?> GetOrderInvoiceAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
            if (order == null)
                return null;

            var invoice = order.Invoices.FirstOrDefault();
            var orderTable = order.Ordertables.FirstOrDefault();

            var orderDetails = await _orderRepository.GetOrderItemsAsync(orderId);
            decimal subTotal =
                orderDetails.Sum(i => i.Total) + orderDetails.Sum(i => i.ModifierTotal);

            // Get tax mappings from repository
            var taxMappings = await _orderTaxRepository.GetTaxMappingsByOrderIdAsync(orderId);

            var taxBreakdown = taxMappings
                .Select(
                    t =>
                        new TaxBreakdownViewModel
                        {
                            TaxName = t.Taxid == 0 ? "Other" : t.Tax?.Taxname ?? "Unknown",
                            TaxValue = (decimal)(t.Taxvalue ?? 0),
                        }
                )
                .ToList();

            decimal totalTax = taxBreakdown.Sum(t => t.TaxValue);
            decimal totalAmountDue = subTotal + totalTax;

            var sections = order.Ordertables
                .Where(ot => ot.Table?.Section != null)
                .Select(ot => ot.Table?.Section?.Sectionname)
                .Where(sectionName => sectionName != null)
                .Distinct()
                .ToList();

            if (sections == null)
                throw new Exception("Sections are empty");

            var tables = order.Ordertables
                .Where(ot => ot.Table != null)
                .Select(ot => ot.Table.Tablename)
                .Distinct()
                .ToList();

            return new OrderInvoiceViewModel
            {
                OrderId = order.Orderid,
                OrderStatus = ((OrderStatus)order.Status).ToString(),
                CustomerName = order.Customer?.Customername ?? "Unknown Customer",
                CustomerPhone = order.Customer?.Phoneno ?? "N/A",
                CustomerEmail = order.Customer?.Email ?? "N/A",
                NoOfPersons = order.Noofperson ?? 0,
                InvoiceNumber = order.InvoiceNumber ?? "N/A",
                PaidOn = order.Modifiedat,
                OrderDate = order.Createdat ?? DateTime.MinValue,
                ModifiedOn = order.Modifiedat ?? DateTime.MinValue,
                OrderDuration = CalculateOrderDuration(order.Createdat, order.Modifiedat),
                Sections = sections.Select(s => s ?? string.Empty).ToList(),
                Tables = tables,
                Items = orderDetails,
                SubTotal = subTotal,
                TotalAmountDue = totalAmountDue,
                TaxBreakdown = taxBreakdown, // <-- new
                PaymentMethod = order.Paymentmode ?? "Pending"
            };
        }
        private string CalculateOrderDuration(DateTime? orderDate, DateTime? modifiedOn)
        {
            if (orderDate.HasValue && modifiedOn.HasValue)
            {
                TimeSpan duration = modifiedOn.Value - orderDate.Value;
                int hours = (int)duration.TotalHours;
                int minutes = duration.Minutes;
                return $"{hours} Hours {minutes} Minutes";
            }
            return "0 Hours 0 Minutes";
        }
        #endregion


        #region  CalculateTax
        private static decimal CalculateTax(
            decimal subTotal,
            IEnumerable<Taxis> taxes,
            string taxName
        )
        {
            var tax = taxes?.FirstOrDefault(t => t.Taxname == taxName);
            if (tax == null)
                return 0M;

            return tax.Taxtype == "Percentage"
              ? (subTotal * (decimal)tax.Taxvalue / 100)
              : (decimal)tax.Taxvalue;
        }
        #endregion

        #region  GetKOTDetailsAsync
        public async Task<PaginatedList<KOTViewModel>> GetKOTDetailsAsync(
            int pageNumber,
            int pageSize,
            int categoryId,
            string itemStatus,
            string orderStatus = ""
        )
        {
            var orders = await _orderRepository.GetOrdersWithDetailsAsync();
            var itemsDict = await _orderRepository.GetItemDictionaryAsync();
            var modifiersDict = await _orderRepository.GetModifierDictionaryAsync();

            if (categoryId != 0)
            {
                orders = orders
                    .Where(
                        o =>
                            o.Ordereditems.Any(
                                oi =>
                                    itemsDict.TryGetValue(oi.Itemid, out var item)
                                    && item.CategoryId == categoryId
                            )
                    )
                    .ToList();
            }

            if (!string.IsNullOrEmpty(itemStatus))
            {
                if (orderStatus == "InProgress")
                {
                    orders = orders
                        .Where(
                            o =>
                                o.Ordereditems.Any(oi => (oi.Quantity - oi.Readyquantity) > 0)
                                && o.Status == 1
                        )
                        .ToList();
                }
                else if (orderStatus == "Ready")
                {
                    orders = orders
                        .Where(o => o.Ordereditems.Any(oi => oi.Readyquantity > 0 && o.Status == 1))
                        .ToList();
                }
            }

            var totalOrders = orders.Count;

            orders = orders
                .OrderBy(o => o.Orderid)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = orders
                .Select(
                    o =>
                        new KOTViewModel
                        {
                            OrderNumber = o.Orderid,
                            SectionNames = o.Ordertables
                                .Where(ot => ot.Table?.Section != null)
                                .Select(ot => ot.Table.Section!.Sectionname)
                                .Distinct()
                                .ToList(),
                            TableNames = o.Ordertables
                                .Where(ot => ot.Table != null)
                                .Select(ot => ot.Table.Tablename)
                                .Distinct()
                                .ToList(),
                            ElapsedTime = GetElapsedTime(o.Createdat ?? DateTime.MinValue),
                            CreatedAt = o.Createdat?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A",
                            CreatedAtRaw = o.Createdat,
                            Status = orderStatus,
                            SpecialInstructions = o.Orderwisecomment ?? "No special instructions",
                            OrderItems = o.Ordereditems
                                .Where(
                                    oi =>
                                        itemStatus == "InProgress"
                                            ? (oi.Quantity - oi.Readyquantity) > 0
                                            : oi.Readyquantity > 0
                                )
                                .ToList()
                                .Select(
                                    oi =>
                                        new OrderItemViewModel
                                        {
                                            ItemName = itemsDict.TryGetValue(
                                                oi.Itemid,
                                                out var item
                                            )
                                                ? item.Name
                                                : "Unknown Item",
                                            OrderItemId = oi.Ordereditemid,
                                            Quantity =
                                                itemStatus == "InProgress"
                                                    ? (oi.Quantity - oi.Readyquantity)!
                                                    : oi.Readyquantity,
                                            ReadyQuantity = oi.Readyquantity,
                                            ItemWiseComment = oi.Itemwisecomment ?? "No comment",
                                            UnitPrice = itemsDict.TryGetValue(
                                                oi.Itemid,
                                                out var itemInfo
                                            )
                                                ? itemInfo.Rate
                                                : 0,
                                            Total =
                                                oi.Quantity
                                                * (
                                                    itemsDict.TryGetValue(
                                                        oi.Itemid,
                                                        out var itemRate
                                                    )
                                                        ? itemRate.Rate
                                                        : 0
                                                ),
                                            ModifierTotal = oi.Ordereditemmodifers
                                                .Where(
                                                    m => modifiersDict.ContainsKey(m.Itemmodifierid)
                                                )
                                                .Sum(m => modifiersDict[m.Itemmodifierid].Rate),
                                            Modifiers = oi.Ordereditemmodifers
                                                .Where(
                                                    m => modifiersDict.ContainsKey(m.Itemmodifierid)
                                                )
                                                .Select(
                                                    m =>
                                                        new ModifierViewModel
                                                        {
                                                            Modifiername =
                                                                modifiersDict[
                                                                    m.Itemmodifierid
                                                                ].Name,
                                                            Rate =
                                                                modifiersDict[m.Itemmodifierid].Rate
                                                        }
                                                )
                                                .ToList()
                                        }
                                )
                                .ToList()
                        }
                )
                .Where(k => k.OrderItems.Any())
                .ToList();

            return new PaginatedList<KOTViewModel>(result, totalOrders, pageNumber, pageSize);
        }
        #endregion


        #region  CreatOrderAsync
        public async Task<int> CreateOrderAsync(OrderViewModel viewModel)
        {
            try
            {
                var order = new Order
                {
                    Customerid = viewModel.CustomerId ?? 0,
                    Status = (int)viewModel.Status,
                    Noofperson = (short?)viewModel.NoOfPersons,
                };

                return await _orderRepository.CreateOrderAsync(order);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "An error occurred while creating the order. Details: " + ex.Message,
                    ex
                );
            }
        }
        #endregion

        #region  GetCustommerSummary
        public async Task<OrderInvoiceViewModel> GetCustomerSummary(int orderId)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
            if (order == null)
                return new OrderInvoiceViewModel();

            var invoice = order.Invoices.FirstOrDefault();
            var tableNames = order.Ordertables
                .Select(ot => ot.Table?.Tablename)
                .Where(name => !string.IsNullOrEmpty(name))
                .Select(name => name!)
                .ToList();

            var sectionNames = order.Ordertables
                .Select(ot => ot.Table?.Section?.Sectionname)
                .Where(name => !string.IsNullOrEmpty(name))
                .Select(name => name!)
                .Distinct()
                .ToList();


            decimal totalTableCapacity = order.Ordertables
                .Where(ot => ot.Table != null)
                .Sum(ot => ot.Table.Capacity);

            var orderDetails = await _orderRepository.GetOrderItemsAsync(orderId);
            decimal subTotal =
                orderDetails.Sum(i => i.Total) + orderDetails.Sum(i => i.ModifierTotal);

            var taxes = _taxfees.GetAll();

            decimal cgst = CalculateTax(subTotal, taxes, "CGST");
            decimal sgst = CalculateTax(subTotal, taxes, "SGST");
            decimal gst = CalculateTax(subTotal, taxes, "GST");
            decimal other = CalculateTax(subTotal, taxes, "Other");

            decimal totalAmountDue = subTotal + cgst + sgst + gst + other;

            return new OrderInvoiceViewModel
            {
                OrderId = order.Orderid,
                OrderStatus = ((OrderStatus)order.Status).ToString(),
                CustomerName = order.Customer?.Customername ?? "Unknown Customer",
                CustomerPhone = order.Customer?.Phoneno ?? "N/A",
                CustomerEmail = order.Customer?.Email ?? "N/A",
                CustomerId = (int)order.Customer?.Customerid!,
                NoOfPersons = order.Noofperson ?? 0,
                TableCapacity = totalTableCapacity,
                InvoiceNumber = invoice?.Invoicenumber ?? "N/A",
                PaidOn = DateTime.UtcNow,
                OrderDate = invoice?.Invoicedate ?? DateTime.MinValue,
                ModifiedOn = order.Modifiedat ?? DateTime.MinValue,
                OrderDuration = "0 Hours 0 Minutes",
                Tables = tableNames,
                Sections = sectionNames,
                Items = orderDetails,
                SubTotal = subTotal,
                CGST = cgst,
                SGST = sgst,
                GST = gst,
                Other = other,
                TotalAmountDue = totalAmountDue,
                PaymentMethod = order.Paymentmode ?? "Unknown"
            };
        }
        #endregion

        #region  GetElapsedTime
        private static string GetElapsedTime(DateTime createdTime)
        {
            var elapsed = DateTime.Now - createdTime;
            return $"{(int)elapsed.TotalMinutes} min ago";
        }
        #endregion

        #region  SaveOrderComment
        public async Task<bool> SaveOrderComment(int orderId, string comment)
        {
            // return await _orderRepository.SaveOrderComment(orderId, comment);
            return await _orderRepository.SaveOrderCommentSP(orderId, comment);
        }
        #endregion

        #region  GetOrderByOrderId
        public async Task<OrderCommentViewModel?> GetOrderById(int orderId)
        {
            // var order = await _orderRepository.GetOrderById(orderId);
            var order = await _orderRepository.GetOrderByIdSP(orderId);
            if (order == null)
            {
                return null; // Return null if the order is not found
            }
            return new OrderCommentViewModel
            {
                OrderId = order.Orderid,
                Comment = order.Orderwisecomment,
            };
        }
        #endregion

        #region  GetSpecialInstruction
        public async Task<string> GetSpecialInstruction(int orderId, int orderedItemId)
        {
            return await _orderRepository.GetSpecialInstruction(orderId, orderedItemId);
        }
        #endregion

        #region  SaveSpecialInstruction 
        public async Task<bool> SaveSpecialInstruction(
            int orderId,
            int orderedItemId,
            string instruction
        )
        {
            return await _orderRepository.SaveSpecialInstruction(
                orderId,
                orderedItemId,
                instruction
            );
        }
        #endregion

        #region  GetOrderItemsAsynce
        public async Task<List<OrderItemViewModel>> GetOrderItemsAsync(int orderId)
        {
            var orderedItems = await _orderRepository.GetOrderedItemsAsync(orderId);

            if (!orderedItems.Any())
                return new List<OrderItemViewModel>();

            var orderedItemIds = orderedItems.Select(oi => oi.Itemid).ToList();
            var itemIds = orderedItems.Select(oi => oi.Itemid).Distinct().ToList();

            var items = await _orderRepository.GetItemsByIdsAsync(itemIds);

            var orderedModifiers = await _orderRepository.GetOrderedItemModifiersAsync(orderId);

            var modifierIds = orderedModifiers
                .Select(oim => oim.Itemmodifierid)
                .Distinct()
                .ToList();
            var modifiers = await _orderRepository.GetModifiersByIdsAsync(modifierIds);

            var orderItemViewModels = orderedItems
                .Select(
                    oi =>
                    {
                        var item = items.FirstOrDefault(i => i.Itemid == oi.Itemid);

                        var itemModifiers = orderedModifiers
                            .Where(oim => oim.Ordereditemid == oi.Ordereditemid)
                            .Select(
                                oim =>
                                {
                                    var modifier = modifiers.FirstOrDefault(
                                        m => m.Modifierid == oim.Itemmodifierid
                                    );
                                    if (modifier == null)
                                        return null;

                                    return new ModifierViewModel
                                    {
                                        ModifierId = oim.Itemmodifierid,
                                        Modifiername = modifier?.Modifiername ?? "Unknown Modifier",
                                        Rate = modifier?.Rate ?? 0,
                                        Quantity = oim.Quantity,
                                        Orderid = oim.Orderid,
                                        Ordereditemid = oim.Ordereditemid,
                                    };
                                }
                            )
                            .ToList();


                        decimal modifiersTotal = itemModifiers.Sum(
                            m => ((decimal?)m!.Rate ?? 0) * (m.Quantity ?? 0)
                        );

                        return new OrderItemViewModel
                        {
                            OrderItemId = oi.Ordereditemid,
                            OrderId = oi.Orderid,
                            ItemId = oi.Itemid,
                            ReadyQuantity = oi.Readyquantity,
                            ItemName = item?.Itemname,
                            Quantity = oi.Quantity,
                            UnitPrice = item?.Rate ?? 0,
                            Total = (oi.Quantity * (item?.Rate ?? 0)) + modifiersTotal,
                            ModifierTotal = modifiersTotal,
                            Modifiers = itemModifiers!
                        };
                    }
                )
                .ToList();

            return orderItemViewModels;
        }
        #endregion

        #region  SaveOrder
        public async Task SaveOrder(OrderRequestModel orderRequest)
        {
            var order = await _orderRepository.GetByIdAsync(orderRequest.OrderId)
                ?? throw new Exception("Order not found.");

            try
            {
                order.Status = (int)OrderStatus.InProgress;
                order.Subamount = (decimal?)orderRequest.SubAmount;
                order.Discount = (decimal?)orderRequest.Discount;
                order.Totaltax = (decimal?)orderRequest.TotalTax;
                order.Totalamount = (decimal)orderRequest.TotalAmount;
                order.Paymentmode = orderRequest.PaymentMethod;

                await _orderRepository.UpdateAsync(order);

                var currentItems = await _orderItemRepository.GetItemsByOrderIdAsync(orderRequest.OrderId);

                var currentItemGroups = new List<(Ordereditem item, List<Ordereditemmodifer> modifiers)>();
                foreach (var existingItem in currentItems)
                {
                    var modifiers = await _orderItemModifierRepository.GetModifierMappingsByOrderedItemIdAsync(existingItem.Ordereditemid);
                    currentItemGroups.Add((existingItem, modifiers));
                }

                var matchedOrderedItemIds = new HashSet<int>();

                if (orderRequest.Items == null)
                    throw new Exception("No Items to manipulate");
                foreach (var item in orderRequest.Items)
                {
                    bool matched = false;

                    foreach (var (dbItem, dbModifiers) in currentItemGroups)
                    {
                        if (item.Modifiers == null)
                            throw new Exception("Added items are null");
                        bool modifiersMatch = item.Modifiers.Count == dbModifiers.Count &&
                            item.Modifiers.All(mod => dbModifiers.Any(db =>
                                db.Itemmodifierid == mod.ModifierId && db.Quantity == item.Quantity));

                        bool itemsMatch = item.Modifiers.Count == dbModifiers.Count &&
                            item.Modifiers.All(mod => dbModifiers.Any(db =>
                                db.Itemmodifierid == mod.ModifierId && db.Quantity != item.Quantity));

                        if (item.ItemId == dbItem.Itemid && itemsMatch)
                        {
                            dbItem.Quantity = item.Quantity;
                            await _orderItemRepository.UpdateAsync(dbItem);
                            matchedOrderedItemIds.Add(dbItem.Ordereditemid);
                            matched = true;
                            break;
                        }
                        if (item.ItemId == dbItem.Itemid && modifiersMatch)
                        {
                            dbItem.Quantity = item.Quantity;
                            await _orderItemRepository.UpdateAsync(dbItem);
                            matchedOrderedItemIds.Add(dbItem.Ordereditemid);
                            matched = true;
                            break;
                        }
                    }

                    if (!matched)
                    {
                        var newItem = new Ordereditem
                        {
                            Orderid = orderRequest.OrderId,
                            Itemid = item.ItemId,
                            Quantity = item.Quantity
                        };

                        await _orderItemRepository.Add(newItem);

                        var addedItem = await _orderItemRepository.GetByIdAsync(newItem.Ordereditemid);
                        if (addedItem == null)
                            throw new Exception("Added items are null");
                        if (item.Modifiers == null)
                            throw new Exception("Added items are null");
                        foreach (var mod in item.Modifiers)
                        {
                            var newMod = new Ordereditemmodifer
                            {
                                Ordereditemid = addedItem.Ordereditemid,
                                Quantity = item.Quantity,
                                Itemmodifierid = mod.ModifierId,
                                Orderid = orderRequest.OrderId
                            };
                            await _orderItemModifierRepository.AddOrderedItemModifierMappingIfNotExistsAsync(newMod);
                        }

                        matchedOrderedItemIds.Add(addedItem.Ordereditemid);
                    }
                }

                foreach (var (dbItem, _) in currentItemGroups)
                {
                    if (!matchedOrderedItemIds.Contains(dbItem.Ordereditemid))
                    {
                        await _orderItemRepository.DeleteAsync(dbItem.Ordereditemid);
                        await _orderItemModifierRepository.DeleteAllModifierMappingsForOrderedItemIdAsync(dbItem.Ordereditemid);
                    }
                }

                if (orderRequest.Taxes != null && orderRequest.Taxes.Any())
                {
                    await _orderTaxRepository.DeleteTaxMappingsByOrderIdAsync(orderRequest.OrderId);
                    foreach (var tax in orderRequest.Taxes)
                    {
                        var orderTax = new Ordertaxmapping
                        {
                            Orderid = orderRequest.OrderId,
                            Taxid = tax.TaxId,
                            Taxvalue = tax.TaxValue
                        };
                        await _orderTaxRepository.AddTaxMappingToOrderAsync(orderTax);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region  CheckReadyQuantityAsynce
        public async Task<string> CheckReadyQuantityAsync(int orderedItemId)
        {
            var item = await _orderItemRepository.GetByIdAsync(orderedItemId);

            if (item == null)
            {
                return "Item not found.";
            }

            if (item.Readyquantity != 0)
            {
                return $"Cannot delete item with ID {orderedItemId} because ReadyQuantity is not 0.";
            }

            return "Item is ready for deletion.";
        }
        #endregion

        #region  MarkOrderAsCompleteAsynce
        public async Task<bool> MarkOrderAsCompleteAsync(int orderId)
        {
            var order =
                await _orderRepository.GetByIdAsync(orderId)
                ?? throw new Exception("Order not found.");

            if (order == null)
                return false;
            order.Status = (int)OrderStatus.Completed;


            var invoiceNumber = GenerateInvoiceNumber(orderId);
            order.InvoiceNumber = invoiceNumber;
            if (order.Customer != null)
            {
                order.Customer.Totalorder++;
                await _customerRepository.UpdateCustomerAsync(order.Customer);
            }
            await _orderRepository.UpdateAsync(order);

            foreach (var mapping in order.Ordertables)
            {
                if (mapping.Table != null)
                {
                    mapping.Table.Tablestatus = 1;
                }
            }

            return await _orderRepository.SaveChangesAsync();
        }
        #endregion

        #region  CancelOrderAsync
        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order =
                await _orderRepository.GetByIdAsync(orderId)
                ?? throw new Exception("Order not found.");

            if (order == null)
                return false;

            order.Status = (int)OrderStatus.Cancelled;
            foreach (var mapping in order.Ordertables)
            {
                if (mapping.Table != null)
                {
                    mapping.Table.Tablestatus = 1;
                }
            }

            await _orderRepository.UpdateAsync(order);

            return true;
        }
        #endregion

        #region  GeneratInvoiceNumber
        private static string GenerateInvoiceNumber(int orderId)
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            return $"{datePart}-{orderId}";
        }
        #endregion

        public async Task<PaginatedList<KOTViewModel>> GetKOTDetailsAsyncSP(int pageNumber, int pageSize, int categoryId, string itemStatus, string orderStatus = "")
        {
            var flatData = await _orderRepository.GetKOTDataFromProcedureAsync(pageNumber, pageSize, categoryId, orderStatus, itemStatus);

            var grouped = flatData
                .GroupBy(x => x.OrderId)
                .Select(orderGroup => new KOTViewModel
                {
                    OrderNumber = orderGroup.Key,
                    SectionNames = orderGroup.Select(x => x.SectionName ?? string.Empty).Distinct().ToList(),
                    TableNames = orderGroup.Select(x => x.TableName ?? string.Empty).Distinct().ToList(),
                    CreatedAt = orderGroup.First().CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty,
                    CreatedAtRaw = orderGroup.First().CreatedAt,
                    Status = orderStatus,
                    SpecialInstructions = orderGroup.First().OrderwiseComment ?? "No special instructions",
                    OrderItems = orderGroup
                        .GroupBy(i => i.ItemId)
                        .Select(itemGroup => new OrderItemViewModel
                        {
                            OrderItemId = itemGroup.First().OrderItemId,
                            ItemName = itemGroup.First().ItemName,
                            Quantity = itemStatus == "InProgress"
                                ? itemGroup.First().Quantity - itemGroup.First().ReadyQuantity
                                : itemGroup.First().ReadyQuantity,
                            ReadyQuantity = itemGroup.First().ReadyQuantity,
                            ItemWiseComment = itemGroup.First().ItemwiseComment ?? "No comment",
                            UnitPrice = itemGroup.First().ItemRate,
                            Total = itemGroup.First().ItemRate * itemGroup.First().Quantity,
                            ModifierTotal = itemGroup
                                .Where(x => !string.IsNullOrEmpty(x.ModifierName))
                                .Sum(x => x.ModifierRate ?? 0),
                            Modifiers = itemGroup
                                .Where(x => !string.IsNullOrEmpty(x.ModifierName))
                                .Select(x => new ModifierViewModel
                                {
                                    Modifiername = x.ModifierName ?? "",
                                    Rate = x.ModifierRate ?? 0
                                }).ToList()
                        }).ToList()
                }).Where(k => k.OrderItems.Any()).ToList();

            var totalCount = grouped.Count;

            return new PaginatedList<KOTViewModel>(grouped, totalCount, pageNumber, pageSize);
        }

    }
}
