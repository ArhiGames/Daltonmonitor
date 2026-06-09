
window.addEventListener("load", () => {
    checkVisibleDays()
    setInterval(checkVisibleDays, 1000 * 60 * 15);
})

function checkVisibleDays() {
    console.log("Checking for past days...")
    const currentDate = new Date()
    const currentDateString = String(currentDate.getFullYear() + addLeadingZero(currentDate.getMonth() + 1) + addLeadingZero(currentDate.getDate()))
    const currentDateNumber =  Number(currentDateString)
    console.log(`Current date is: ${currentDateNumber}`)
    
    let countVisibleDays = 0
    document.querySelectorAll(".day").forEach(day => {
        if (Number(day.dataset.date) < currentDateNumber || countVisibleDays >= 2) {
            day.style.display = "none"
        } else {
            countVisibleDays++
        }
    })
}

function addLeadingZero(number) {
    if (String(number).length === 2) {
        return String(number)
    }
    return "0" + number
}