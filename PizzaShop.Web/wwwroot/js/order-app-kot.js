$(document).ready(function () {
  let currentPage = parseInt($("#currentPage").val()) || 1;
  let selectedCategoryId = $(".category-tab.active").data("category-id") || 0;
  let selectedOrderStatus = "InProgress";

  loadKOT(
    currentPage,
    selectedCategoryId,
    selectedOrderStatus,
    selectedOrderStatus
  );

  $(document).on("click", ".category-tab", function () {
    $(".category-tab").removeClass("active");
    $(this).addClass("active");

    selectedCategoryId = $(this).data("category-id");
    $("#tab-heading").text($(this).find(".custom-text").text());
    updatePaginationControls();
    currentPage = 1;
    loadKOT(
      currentPage,
      selectedCategoryId,
      selectedOrderStatus,
      selectedOrderStatus
    );
  });

  $(document).on("click", ".btn-order-status", function () {
    $(".btn-order-status")
      .removeClass("btn-primary")
      .addClass("btn-outline-primary");

    $(this).removeClass("btn-outline-primary").addClass("btn-primary");

    selectedOrderStatus = $(this).text();

    updatePaginationControls();
    loadKOT(1, selectedCategoryId, selectedOrderStatus, selectedOrderStatus);
  });

  function loadKOT(pageNumber, categoryId, orderStatus, itemStatus) {
    $("#currentPage").val(pageNumber);

    $.ajax({
      url: `/OrderAppKOT/Index/${categoryId}/${pageNumber}?orderStatus=${orderStatus}&itemStatus=${itemStatus}`,
      type: "GET",
      success: function (data) {
        $("#KOTContainer").html(data);

        let currentPage = parseInt($("#currentPage").val()) || 1;
        let totalPages = parseInt($("#totalPages").val()) || 1;

        updatePaginationControls(totalPages);
        $(".kot-time").each(function () {
          var kotId = $(this).attr("id").split("_")[1];
          var createdAt = $(this).data("created-at");
        });
      },
      error: function (xhr, status, error) {
        toastr.error("AJAX Error: " + status + " - " + error);
        toastr.info(xhr.responseText);
      },
    });
  }

  function updateItemStatuses(orderStatus) {
    if (orderStatus === "Ready") {
      $(".kot-card").each(function () {
        const orderStatus = $(this).data("status");
        if (orderStatus === "InProgress") {
          $(this).find(".status-text").text("Ready");
          $(this).addClass("ready-state");
        }
      });
    }
  }

  $(document).on("click", ".pagination-link", function () {
    var pageNumber = $(this).data("page");
    if (pageNumber) {
      loadKOT(
        pageNumber,
        selectedCategoryId,
        selectedOrderStatus,
        selectedOrderStatus
      );
    }
  });

  $(document).on("click", "#prevPageBtn", function () {
    var currentPage = parseInt($("#currentPage").val());
    if (currentPage > 1) {
      loadKOT(
        currentPage - 1,
        selectedCategoryId,
        selectedOrderStatus,
        selectedOrderStatus
      );
    }
  });

  $(document).on("click", "#nextPageBtn", function () {
    var currentPage = parseInt($("#currentPage").val());
    var totalPages = parseInt($("#totalPages").val());
    if (currentPage < totalPages) {
      loadKOT(
        currentPage + 1,
        selectedCategoryId,
        selectedOrderStatus,
        selectedOrderStatus
      );
    }
  });

  function updatePaginationControls(totalPages) {
    var currentPage = parseInt($("#currentPage").val()) || 1;
    $("#prevPageBtn").prop("disabled", currentPage <= 1);
    $("#nextPageBtn").prop("disabled", currentPage >= totalPages);
  }

  $(document).on("click", ".kot-card", function () {
    const orderId = $(this).find(".text-primary").text().trim();
    const orderItems = $(this).find("ul.list-unstyled > li");
    const orderStatus = $(this).data("status");

    $("#orderId").text(orderId);

    $("#orderItems").empty();
    $("#orderStatus").val(orderStatus);

    orderItems.each(function () {
      const itemName = $(this).find("div:first").text().trim();
      const quantity = $(this).find("div:last").text().trim();
      const maxQuantity = parseInt(quantity);
      const orderedItemId = $(this).data("ordereditemid");

      const modifiers = $(this)
        .next("ul.custom-li")
        .find("li")
        .map(function () {
          return $(this).text().trim();
        })
        .get();

      $("#orderItems").append(`
<tr>
<td>
    <input type="checkbox" class="item-checkbox" data-ordereditemid="${orderedItemId}">
</td>
<td>
    <div>${itemName}</div>
    ${
      modifiers.length > 0
        ? `<div class="text-muted small flex-nowrap">・ ${modifiers.join(
            ", "
          )}</div>`
        : ""
    }
</td>
<td>
    <div class="quantity-box" data-max="${maxQuantity}">
        <button class="decrease-quantity">-</button>
        <span>${maxQuantity}</span>
        <button class="increase-quantity">+</button>
    </div>
</td>
</tr>
`);
    });
    $("#orderDetailsModal").modal("show");
    updateActionButtonText(orderStatus);
  });

  function updateActionButtonText(orderStatus) {
    var orderActionButton = $("#orderActionBtn");

    if (orderActionButton.length > 0) {
      orderActionButton.removeClass("in-progress ready unavailable");

      if (orderStatus == "InProgress") {
        orderActionButton
          .text("Mark as Prepared")
          .prop("disabled", true)
          .removeClass("markAsInProgress")
          .addClass("markAsPrepared");
      } else if (orderStatus == "Ready") {
        orderActionButton
          .text("Mark as InProgess")
          .prop("disabled", true)
          .removeClass("markAsPrepared")
          .addClass("markAsInProgress");
      } else {
        orderActionButton
          .text("Action Unavailable")
          .prop("disabled", true)
          .addClass("unavailable");
      }
    } else {
      toastr.error("The button #orderActionBtn does not exist.");
    }
  }

  $(document).on("change", ".item-checkbox", function () {
    const anyChecked = $(".item-checkbox:checked").length > 0;
    $(".markAsPrepared").prop("disabled", !anyChecked);
  });
  $(document).on("change", ".item-checkbox", function () {
    const anyChecked = $(".item-checkbox:checked").length > 0;
    $(".markAsInProgress").prop("disabled", !anyChecked);
  });

  $(".markAsPrepared").on("click", function () {
    const orderId = $("#orderId").text();
    $("#orderDetailsModal").modal("hide");
  });
  $(".markAsInProgress").on("click", function () {
    const orderId = $("#orderId").text();
    $("#orderDetailsModal").modal("hide");
  });

  $(document).on("click", ".increase-quantity", function () {
    const quantityBox = $(this).closest(".quantity-box");
    const quantitySpan = quantityBox.find("span");
    let quantity = parseInt(quantitySpan.text());
    const max = parseInt(quantityBox.data("max"));

    if (quantity < max) {
      quantitySpan.text(quantity + 1);
    }
  });

  $(document).on("click", ".decrease-quantity", function () {
    const quantitySpan = $(this).siblings("span");
    let quantity = parseInt(quantitySpan.text());
    if (quantity > 1) {
      quantitySpan.text(quantity - 1);
    }
  });

  $(document).on("click", ".markAsPrepared", function () {
    const orderId = $("#orderId").text();
    const updates = [];

    $("#orderItems tr").each(function () {
      const checkbox = $(this).find(".item-checkbox");
      if (checkbox.is(":checked")) {
        const orderedItemId = checkbox.data("ordereditemid");
        const quantity = parseInt($(this).find(".quantity-box span").text());

        updates.push({
          orderedItemId: orderedItemId,
          readyQuantity: quantity,
        });
      }
    });
    if (updates.length === 0) {
      toastr.warning(
        "Please select at least one item to mark as prepared.",
        "warning"
      );
      return;
    }
    $.ajax({
      url: "/OrderAppKOT/UpdateReadyQuantities",
      type: "POST",
      contentType: "application/json",
      data: JSON.stringify(updates),
      success: function (response) {
        if (response.success) {
          toastr.success(response.message);
          $("#orderDetailsModal").modal("hide");
          currentPage = 1;
          loadKOT(
            currentPage,
            selectedCategoryId,
            selectedOrderStatus,
            selectedOrderStatus
          );
          updatePaginationControls();
        } else {
          toastr.error(response.message);
        }
      },
    });
  });

  $(document).on("click", ".markAsInProgress", function () {
    const orderId = $("#orderId").text();
    const updates = [];

    $("#orderItems tr").each(function () {
      const checkbox = $(this).find(".item-checkbox");
      if (checkbox.is(":checked")) {
        const orderedItemId = checkbox.data("ordereditemid");
        const quantity = parseInt($(this).find(".quantity-box span").text());

        updates.push({
          orderedItemId: orderedItemId,
          readyQuantity: quantity,
        });
      }
    });
    if (updates.length === 0) {
      toastr.warning(
        "Please select at least one item to mark as prepared.",
        "warning"
      );
      return;
    }
    $.ajax({
      url: "/OrderAppKOT/UpdateQuantities",
      type: "POST",
      contentType: "application/json",
      data: JSON.stringify(updates),
      success: function (response) {
        if (response.success) {
          toastr.success(response.message);
          $("#orderDetailsModal").modal("hide");
          currentPage = 1;
          loadKOT(
            currentPage,
            selectedCategoryId,
            selectedOrderStatus,
            selectedOrderStatus
          );
          updatePaginationControls();
        } else {
          toastr.error("An error occurred while updating quantities.");
        }
      },
    });
  });
  function updateElapsedTimes() {
    const elements = document.querySelectorAll(".elapsed-time");
    const now = new Date();

    elements.forEach((el) => {
      const createdTime = new Date(el.dataset.time);
      let diffInSeconds = Math.floor((now - createdTime) / 1000);

      const days = Math.floor(diffInSeconds / (3600 * 24));
      diffInSeconds %= 3600 * 24;

      const hours = Math.floor(diffInSeconds / 3600);
      diffInSeconds %= 3600;

      const minutes = Math.floor(diffInSeconds / 60);
      const seconds = diffInSeconds % 60;

      const parts = [];
      if (days > 0) parts.push(`${days} day${days > 1 ? "s" : ""}`);
      if (hours > 0) parts.push(`${hours} hour${hours > 1 ? "s" : ""}`);
      if (minutes > 0) parts.push(`${minutes} min`);
      parts.push(`${seconds} sec`);

      el.textContent = `⏱️ ${parts.join(" ")}`;
    });
  }

  updateElapsedTimes();
  setInterval(updateElapsedTimes, 1000);
});
