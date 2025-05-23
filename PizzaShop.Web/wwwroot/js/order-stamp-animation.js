const stampStyles = document.createElement("style");
stampStyles.textContent = `
      /* Stamp Animation Styles */
      .stamp-container {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        pointer-events: none;
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 1000;
      }
      
      .order-stamp {
        width: 240px;
        height: 120px;
        border: 4px solid #1e7e34;
        border-radius: 8px;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        background-color: rgba(40, 167, 69, 0.0);
        transform: translateY(-200px) rotate(12deg);
        opacity: 0;
        pointer-events: none;
      }
      
      .order-stamp.stamping {
        animation: stamp-press 1.5s ease-in-out forwards;
      }
      
      .order-stamp.stamped {
        transform: translateY(0) rotate(0deg);
        opacity: 1;
        background-color: rgba(40, 167, 69, 0.3);
      }
      
      .order-stamp.fading {
        animation: stamp-fade 2s ease-out forwards;
        background-color: rgba(40, 167, 69, 0.3);
      }
      
      .order-stamp .stamp-text {
        color: #1e7e34;
        font-weight: 700;
        letter-spacing: 2px;
      }
      
      .order-stamp .stamp-text-large {
        font-size: 28px;
        margin-bottom: 4px;
      }
      
      .order-stamp .stamp-text-small {
        font-size: 18px;
        border: 2px solid #1e7e34;
        padding: 2px 24px;
        background-color: rgba(40, 167, 69, 0.2);
      }
      
      @keyframes stamp-press {
        0% { transform: translateY(-200px) rotate(12deg); opacity: 0.7; }
        60% { transform: translateY(10px) rotate(5deg); opacity: 0.9; }
        70% { transform: translateY(-5px) rotate(0deg); opacity: 1; }
        75% { transform: translateY(0) rotate(0deg); opacity: 1; background-color: rgba(40, 167, 69, 0.3); }
        80% { transform: translateY(-2px) rotate(0deg); opacity: 1; background-color: rgba(40, 167, 69, 0.3); }
        100% { transform: translateY(0) rotate(0deg); opacity: 1; background-color: rgba(40, 167, 69, 0.3); }
      }
      
      @keyframes stamp-fade {
        0% { opacity: 1; }
        100% { opacity: 0; }
      }
    `;
document.head.appendChild(stampStyles);

function createStampElement() {
  const stampContainer = document.createElement("div");
  stampContainer.className = "stamp-container";
  stampContainer.innerHTML = `
        <div class="order-stamp">
          <div class="stamp-text stamp-text-large">ORDER</div>
          <div class="stamp-text stamp-text-small">COMPLETE</div>
        </div>
      `;
  return stampContainer;
}

function showOrderCompleteStamp() {
  const cardBody = document.querySelector(".card-body");

  if (window.getComputedStyle(cardBody).position === "static") {
    cardBody.style.position = "relative";
  }

  if (cardBody.querySelector(".stamp-container")) return;

  const stampContainer = createStampElement();
  cardBody.appendChild(stampContainer);

  const stampElement = stampContainer.querySelector(".order-stamp");

  stampElement.classList.add("stamping");

  stampElement.addEventListener("animationend", function onStamp() {
    stampElement.classList.remove("stamping");
    stampElement.classList.add("stamped");
    stampElement.removeEventListener("animationend", onStamp);

    setTimeout(() => {
      stampElement.classList.remove("stamped");
      stampElement.classList.add("fading");

      stampElement.addEventListener("animationend", function onFade() {
        stampContainer.remove();
        stampElement.removeEventListener("animationend", onFade);
      });
    }, 1000);
  });
}
