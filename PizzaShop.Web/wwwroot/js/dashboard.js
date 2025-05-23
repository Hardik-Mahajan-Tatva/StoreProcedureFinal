$(document).ready(function () {
  function loadDashboardData() {
    $.ajax({
      url: "/Dashboard/LoadDashbord",
      type: "GET",
      data: {
        timeRange: $("#timeFilter").val(),
        startDate: $("#startDate").val(),
        endDate: $("#endDate").val(),
        userRole: $("#userRoleHidden").val(),
      },
      success: function (response) {
        $("#dashboardContainer").html(response);

        const totalSales = parseFloat($("#totalSales").data("value"));
        const totalOrders = parseInt($("#totalOrders").data("value"));
        const avgOrderValue = parseFloat($("#avgOrderValue").data("value"));
        const avgWaitingTime = parseFloat($("#avgWaitingTime").data("value"));
        const waitingListCount = parseInt($("#waitingListCount").data("value"));
        const newCustomerCount = parseInt($("#newCustomerCount").data("value"));

        animateCounter("totalSales", totalSales, "₹");
        animateCounter("totalOrders", totalOrders);
        animateCounter("avgOrderValue", avgOrderValue, "₹");
        animateCounter("avgWaitingTime", avgWaitingTime, "", " mins");
        animateCounter("waitingListCount", waitingListCount);
        animateCounter("newCustomerCount", newCustomerCount);
        initializeCharts();
      },
      error: function (xhr, status, error) {
        toastr.error("AJAX Error: " + xhr.responseText, "Request Failed");
      },
    });
  }
  function initializeCharts() {
    const revenueElement = document.getElementById("revenueChart");
    const customerElement = document.getElementById("customerChart");

    if (!revenueElement || !customerElement) {
      toastr.error("Chart elements not found in DOM.");
      return;
    }

    const revenueData = JSON.parse(revenueElement.dataset.chartData);
    const customerData = JSON.parse(customerElement.dataset.chartData);

    const revenueCtx = revenueElement.getContext("2d");
    new Chart(revenueCtx, {
      type: "line",
      data: {
        labels: revenueData.map((item) => item.Label),
        datasets: [
          {
            label: "Revenue",
            data: revenueData.map((item) => item.Value),
            backgroundColor: "#EBF2F7",
            borderColor: "#ABCFE3",
            borderWidth: 2,
            tension: 0.3,
            fill: true,
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          y: {
            beginAtZero: true,
            ticks: {
              callback: function (value) {
                return "?" + value.toLocaleString();
              },
            },
          },
        },
      },
    });

    const customerCtx = customerElement.getContext("2d");
    new Chart(customerCtx, {
      type: "line",
      data: {
        labels: customerData.map((item) => item.Label),
        datasets: [
          {
            label: "New Customers",
            data: customerData.map((item) => item.Value),
            backgroundColor: "#EBF2F7",
            borderColor: "#ABCFE3",
            borderWidth: 2,
            tension: 0.3,
            fill: true,
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          y: {
            beginAtZero: true,
            ticks: {
              stepSize: 1,
              callback: function (value) {
                return Number.isInteger(value) ? value : "";
              },
            },
            suggestedMin: 0,
            suggestedMax:
              Math.max(...customerData.map((item) => item.Value)) + 1,
          },
        },
      },
    });
  }
  $("#timeFilter").on("change", function () {
    const selected = $(this).val();

    if (selected === "custom") {
      $("#customDateModal").modal("show");
    } else {
      $("#startDate").val("");
      $("#endDate").val("");
      $("#startDate").closest(".form-floating").addClass("d-none");
      $("#endDate").closest(".form-floating").addClass("d-none");
      loadDashboardData();
    }
  });

  $("#applyCustomDate").on("click", function () {
    let startDate = $("#customStartDate").val();
    let endDate = $("#customEndDate").val();

    if (!startDate || !endDate) {
      toastr.warning("Please select both start and end dates.");
      return;
    }

    $("#startDate").val(startDate);
    $("#endDate").val(endDate);

    $("#customDateModal").modal("hide");
    loadDashboardData();

    $("#customStartDate").val("");
    $("#customEndDate").val("");
  });

  loadDashboardData();
});

function animateCounter(
  id,
  endValue,
  prefix = "",
  suffix = "",
  duration = 1000
) {
  const element = document.getElementById(id);
  if (!element) return;

  let startValue = 0;
  const increment = endValue / (duration / 16);
  const interval = setInterval(() => {
    startValue += increment;
    if (startValue >= endValue) {
      startValue = endValue;
      clearInterval(interval);
    }
    element.innerText =
      prefix + Math.round(startValue).toLocaleString() + suffix;
  }, 16);
}
