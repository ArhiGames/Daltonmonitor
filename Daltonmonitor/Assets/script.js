
const params = new URLSearchParams(window.location.search);
const maxNumberOfDaysToDisplay = Number(params.get("display-days"));
console.log(maxNumberOfDaysToDisplay)
window.addEventListener("load", () => {
    checkVisibleDays();
    setInterval(checkVisibleDays, 1000 * 60 * 15);
})

function checkVisibleDays() {
    console.log("Checking for past days...");
    const currentDate = new Date();
    const currentDateString = String(currentDate.getFullYear() + addLeadingZero(currentDate.getMonth() + 1) + addLeadingZero(currentDate.getDate()));
    const currentDateNumber =  Number(currentDateString);
    console.log(`Current date is: ${currentDateNumber}`);
    
    let countVisibleDays = 0;
    const limitDisplayedDays = !isNaN(maxNumberOfDaysToDisplay) && maxNumberOfDaysToDisplay > 0;
    document.querySelectorAll(".day").forEach(day => {
        day.classList.remove("hidden");
        if (Number(day.dataset.date) < currentDateNumber || (countVisibleDays >= maxNumberOfDaysToDisplay && limitDisplayedDays)) {
            day.classList.add("hidden");
        } else {
            countVisibleDays++;
        }
    })
    if (countVisibleDays === 1) {
        document.body.classList.add("single-day");
    }
}

function addLeadingZero(number) {
    if (String(number).length === 2) {
        return String(number);
    }
    return "0" + number;
}