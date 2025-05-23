document.addEventListener("DOMContentLoaded", function () {
  showRandomPizzaQuote();
  const progressBar = document.getElementById("progress");
  const loader = document.getElementById("loader");
  const mainContent = document.getElementById("main-content");

  let loadedItems = 0;
  let totalItems = 0;

  totalItems =
    document.images.length +
    document.querySelectorAll("script").length +
    document.querySelectorAll('link[rel="stylesheet"]').length +
    1;

  function updateProgress() {
    loadedItems++;
    const percentComplete = Math.min(
      Math.floor((loadedItems / totalItems) * 100),
      100
    );
    progressBar.style.width = percentComplete + "%";

    if (loadedItems >= totalItems) {
      setTimeout(function () {
        loader.style.opacity = "0";
        setTimeout(function () {
          loader.style.display = "none";
          mainContent.style.display = "block";
        }, 300);
      }, 200);
    }
  }

  for (let i = 0; i < document.images.length; i++) {
    const img = document.images[i];
    if (img.complete) {
      updateProgress();
    } else {
      img.addEventListener("load", updateProgress);
      img.addEventListener("error", updateProgress);
    }
  }

  function trackResource(elements) {
    for (let i = 0; i < elements.length; i++) {
      updateProgress();
    }
  }

  trackResource(document.querySelectorAll("script"));
  trackResource(document.querySelectorAll('link[rel="stylesheet"]'));

  updateProgress();

  window.addEventListener("load", function () {
    if (loadedItems < totalItems) {
      loadedItems = totalItems;
      progressBar.style.width = "100%";
      setTimeout(function () {
        loader.style.opacity = "0";
        setTimeout(function () {
          loader.style.display = "none";
          mainContent.style.display = "block";
        }, 300);
      }, 200);
    }
  });
});
const pizzaQuotes = [
  "You can't make everyone happy. You're not pizza.",
  "In crust we trust.",
  "Life is not about finding yourself. It's about finding pizza.",
  "Any time is a good time for pizza.",
  "Pizza is the answer. Who cares what the question is?",
  "There's no 'we' in pizza. Unless you're sharing.",
  "Pizza fixes everything.",
  "In crust we trust.",
  "Pizza is my soulmate.",
  "Love at first slice.",
  "Pizza fixes everything.",
  "Life needs more cheese.",
  "Slice, slice, baby!",
  "Pizza is always right.",
  "Eat, sleep, pizza, repeat.",
  "You had me at pizza.",
  "Keep calm, eat pizza.",
  "Pizza is my spirit animal.",
  "Happiness is extra cheese.",
  "Pizza: a hug in slices.",
  "No drama, just pizza.",
  "Fuelled by pizza today.",
  "Pizza never breaks hearts.",
  "More pizza, less problems.",
  "Cheesy dreams come true.",
  "Pizza is the answer.",
  "Crust me, I'm delicious.",
  "Pizza before people.",
  "Always down for pizza.",
  "The oven never lies.",
  "Powered by pizza slices.",
  "Pizza makes everything okay.",
  "All roads lead to pizza.",
  "Say yes to pizza.",
  "Peace, love, and pizza.",
  "Pizza time is sacred.",
  "A pizza a day!",
];

function showRandomPizzaQuote() {
  const quote = pizzaQuotes[Math.floor(Math.random() * pizzaQuotes.length)];
  document.getElementById("pizza-quote").innerText = quote;
}

function toggle() {
  let input_toggle = document.getElementById("toggle_button_eye");
  let password_input = document.getElementById("custom-password-input");

  if (password_input.type === "password") {
    password_input.type = "text";
    input_toggle.src = "/images/icons/eye.png";
  } else {
    password_input.type = "password";
    input_toggle.src = "/images/icons/hidden.png";
  }
}

// When WebSite Goes Live
// document.addEventListener("DOMContentLoaded", function () {
//   document.addEventListener("contextmenu", function (e) {
//     e.preventDefault();
//   });

//   document.addEventListener("dragstart", function (e) {
//     e.preventDefault();
//   });

//   document.addEventListener("keydown", function (e) {
//     if (
//       e.key === "F12" ||
//       (e.ctrlKey &&
//         e.shiftKey &&
//         ["I", "J", "C", "K"].includes(e.key.toUpperCase())) ||
//       (e.ctrlKey && ["U", "S"].includes(e.key.toUpperCase()))
//     ) {
//       e.preventDefault();
//     }
//   });

//   document.addEventListener("selectstart", function (e) {
//     const target = e.target;
//     if (
//       target.tagName === "INPUT" ||
//       target.tagName === "TEXTAREA" ||
//       target.isContentEditable
//     ) {
//       return;
//     }
//     e.preventDefault();
//   });
// });

// $(document).ready(function () {
//   const removeAds = () => {
//     $("a[href='http://somee.com']").remove();
//     $("div[style='height: 65px;']").remove();
//     $(
//       "div[style*='z-index: 2147483647'][style*='position: fixed'][style*='bottom: 0px']"
//     ).remove();
//   };

//   removeAds();

//   const observer = new MutationObserver(() => {
//     removeAds();
//   });

//   observer.observe(document.body, { childList: true, subtree: true });
// });
