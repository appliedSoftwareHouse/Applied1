
document.getElementById("txtDR").addEventListener("focusout", FormatAmount);


function FormatAmount() {
    
    _DR = document.getElementById("txtDR").value;
    var usFormat = _DR.toLocaleString('en-US');
   
    document.getElementById("txtDR").innerHTML = usFormat;
}