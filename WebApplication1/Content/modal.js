
function showModal(ticket, title) {


    document.getElementById('mt').innerHTML = title;
    document.getElementById('mb').innerHTML = Html.Partial("_TicketModal", ticket);
    $('#myModal').modal('toggle');
}
