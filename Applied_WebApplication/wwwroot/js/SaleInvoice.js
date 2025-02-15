
function calculateTax() {
    var Qty = document.getElementById("txtQty")
    var Rate = document.getElementById("txtRate")
    var Amount = Qty * Rate
}

function calulateAmount() {
    var qty = document.getElementById("txtQty");
    var rate = document.getElementById("txtRate");
    var amount = qty * rate;
    document.getElementById("Qty").innerHTML = amount.toLocaleString("en-US");
   

}

