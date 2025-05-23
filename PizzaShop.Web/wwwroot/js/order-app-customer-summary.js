
function openQrCodeModal() {
  // Get the current URL dynamically
  const currentUrl = window.location.href;

  // Log the current URL for debugging

  // Set the QR code image source with the current URL
  const qrCodeImage = document.getElementById("qrCodeImage");
  qrCodeImage.src = `https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=${encodeURIComponent(
    currentUrl
  )}`;

  // Show the QR code modal
  var qrModal = new bootstrap.Modal(document.getElementById("qrCodeModal"));
  qrModal.show();
}
