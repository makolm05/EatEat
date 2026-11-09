var dataTable;

$(document).ready(() => {
    const params = new URLSearchParams(window.location.search);
    const status = params.get("status");
    loadDataTable(status);
});

function loadDataTable(status){
    dataTable = $('#tblData').DataTable({
        order: [[0, "desc"]],
        "ajax": {
            "url": `/Order/GetAll?status=${status}`
        },
         columns: [
            { data: 'id', width: '5%' },
            { data: 'email', width: '25%' },
            { data: 'name', width: '20%' },
            { data: 'phone', width: '15%' },
            { data: 'status', width: '15%' },
            { data: 'orderTotal', width: '10%' },
            {
                data: 'id',
                render: function (data) {
                    return `<div class="w-75 btn-group" rol="group">
                        <a href="/order/orderDetail?OrderId=${data}" class="bg-warning btn btn-primary mx-2"><i class="bi bi-pencil"></i></a>
                    </div>`
                },
                width: '10%'
            }
        ]
    });
}