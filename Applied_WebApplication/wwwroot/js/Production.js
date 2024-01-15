document.getElementById("_Qty").addEventListener("focusout", myFunction);
document.getElementById("_Rate").addEventListener("focusout", myFunction);


function myFunction() {
    num1 = document.getElementById("_Qty").value;
    num2 = document.getElementById("_Rate").value;
    num3 = num1 * num2;
    document.getElementById("_Amount").innerHTML = num3.toLocaleString('en-US');
}