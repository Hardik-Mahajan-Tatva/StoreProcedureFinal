$(document).ready(function () {
  if (window.TempData) {
    if (TempData.ErrorMessage) {
      toastr.error(TempData.ErrorMessage);
    }
    if (TempData.SuccessMessage) {
      toastr.success(TempData.SuccessMessage);
    }
    if (TempData.WarningMessage) {
      toastr.warning(TempData.WarningMessage);
    }
    if (TempData.InfoMessage) {
      toastr.info(TempData.InfoMessage);
    }
  }
});
$(document).ready(function () {
  $("#Country").change(function () {
    var id = $(this).val();
    if (id) {
      $.ajax({
        url: "/Users/GetStatesByCountry",
        data: { countryId: id },
        success: function (data) {
          var stateDropdown = $("#State");
          stateDropdown.empty();
          stateDropdown.append('<option value="">Select State</option>');

          data.forEach(function (state) {
            stateDropdown.append(
              '<option value="' +
                state.stateid +
                '">' +
                state.statename +
                "</option>"
            );
          });

          $("#City").empty().append('<option value="">Select City</option>');
        },
      });
    } else {
      $("#State").empty().append('<option value="">Select State</option>');
      $("#City").empty().append('<option value="">Select City</option>');
    }
  });

  $("#State").change(function () {
    var id = $(this).val();
    if (id) {
      $.ajax({
        url: "/Users/GetCitiesByState",
        data: { stateId: id },
        success: function (data) {
          var cityDropdown = $("#City");
          cityDropdown.empty();
          cityDropdown.append('<option value="">Select City</option>');

          data.forEach(function (city) {
            cityDropdown.append(
              '<option value="' +
                city.cityid +
                '">' +
                city.cityname +
                "</option>"
            );
          });
        },
      });
    } else {
      $("#City").empty().append('<option value="">Select City</option>');
    }
  });

  window.handleImageUpload = function (input) {
    const file = input.files[0];
    const $preview = $("#imagePreview");
    const $container = $("#imagePreviewContainer");
    const $uploadBtn = $("#uploadButton");

    if (file && file.type.startsWith("image/")) {
      const reader = new FileReader();
      reader.onload = function (e) {
        $preview.attr("src", e.target.result);
        $container.removeClass("d-none");
        $uploadBtn.addClass("d-none");
        $("#RemoveImage").val("false");
      };
      reader.readAsDataURL(file);
    }
  };

  window.editImage = function () {
    $("#inputGroupFile01").click();
  };

  window.removeImage = function () {
    const $input = $("#inputGroupFile01");
    const $preview = $("#imagePreview");
    const $container = $("#imagePreviewContainer");
    const $uploadBtn = $("#uploadButton");

    $input.val("");
    $preview.attr("src", "");
    $container.addClass("d-none");
    $uploadBtn.removeClass("d-none");

    $("#RemoveImage").val("true");
  };
});
