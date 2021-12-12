
function OnDeleteAllClick(obj) { 
    var grid = $find($("div[class*='_CSServiceGrid_']").attr('id'));
    var master = grid.get_masterTableView();
    console.log(master);
    if (window.confirm("Are you sure you want to Clear All data?"))
    { 
        master.fireCommand("DeleteAll", "0");
    } 
} 
$(document).ready(function () { 
    $("input[id*='chkNA']").click(function () { 
        OnNAClick($(this));
     });
    function OnNAClick(obj) { 
        var grid = $find( $("div[class*='_CSServiceGrid_']").attr('id') );
        var master = grid.get_masterTableView(); 
        var checked = $(obj).prop('checked');
        if (checked == false) {
            master.fireCommand("EditAll", ""); // UNSETNA
            return true;
        } else {
            var box = confirm("Any data entered for this metric will be cleared. Are you sure you want to mark this metric as Not Applicable?");
            if (box == true) {  
                master.fireCommand("CancelAll", ""); // SETNA
                $(obj).prop('checked', true);
                return true;
            } else {
                $(obj).prop('checked', false);
            }
        }
    }
}); 

