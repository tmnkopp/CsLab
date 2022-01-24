
function OnDeleteAllClick(sender, args) { 
    var master = sender.get_parent();
    let conf = window.confirm("Are you sure you want to Clear All data?"); 
    if (conf) {
        master.fireCommand("DeleteAll", "0");
    } else {
        args.set_cancel(true);
    }
}
$(document).ready(function () {
    $("input[id*='chkNA']").click(function () {
        let grid = $find($(this).attr('data-grid-id')); 
        OnNAClick($(this), grid);
    });
    function OnNAClick(sender, grid) { 
        var master = grid.get_masterTableView();
        var checked = $(sender).prop('checked');
        if (checked == false) {
            master.fireCommand("EditAll", ""); // UNSETNA
            return true;
        } else {
            var box = confirm("Any data entered for this metric will be cleared. Are you sure you want to mark this metric as Not Applicable?");
            if (box == true) {
                master.fireCommand("CancelAll", ""); // SETNA
                $(sender).prop('checked', true);
                return true;
            } else {
                $(sender).prop('checked', false);
            }
        }
    }
});