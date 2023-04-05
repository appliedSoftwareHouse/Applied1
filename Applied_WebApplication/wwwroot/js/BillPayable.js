document.getElementById("Qty").addEventListener("focusout", myFunction);
document.getElementById("Rate").addEventListener("focusout", myFunction);
document.getElementById("NetAmount").addEventListener("focusin", myFunction2)

document.getElementById("Qty").innerHTML = document.getElementById("Qty").value.toLocaleString("en-US")

function myFunction() {
    num1 = document.getElementById("Qty").value;
    num2 = document.getElementById("Rate").value;
    num3 = num1 * num2;

    document.getElementById("Qty").innerHTML = num1.toLocaleString('en-US');
    document.getElementById("Rate").innerHTML = num2.toLocaleString('en-US');
    document.getElementById("Amount").value = num3.toLocaleString('en-US');
}

function myFunction2() {
    num1 = document.getElementById("Qty").value;
    num2 = document.getElementById("Rate").value;
    num3 = document.getElementById("Tax_Rate").value;
    num4 = num1 * num2;    // Invoice Amount
    num5 = num4 * num3;    // Tax Amount
    num6 = num4 + num5;     // Net Amount
    document.getElementById("NetAmount").value = num6.toLocaleString('en-US');
    document.getElementById("Qty").innerHTML = num1.toLocaleString("en-US"); 
    document.getElementById("Rate").innerHTML = num2=toLocaleString("en-US"); 
    document.getElementById("Tax_Rate").innerHTML = num3=toLocaleString("en-US"); 
}

