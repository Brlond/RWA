let currentPage = 1;
let pageSize = 10;

let logsurl = "http://localhost:8080/api/Log/GetSome";

function fillLogs(page = 1, size = 10) {
    let jwt = localStorage.getItem("JWT");
    console.log(jwt);
    currentPage = page;
    pageSize = size;

    $.ajax({
        url: `${logsurl}?pageNumber=${page}&pageSize=${size}`,
        headers: { "Authorization": `Bearer ${jwt}` }
    }).done(function (data) {
        $("#logContainer").empty();
        drawElements(data);
        drawPagination(data.totalPages);
        console.log(localStorage);

    }).fail(function () {
        console.error("There was an error while trying to load your data");
        window.location.href = "Login.html";
    });
}


function drawElements(logs) {
    const $ph = $("#logContainer");

    for (let card of logs.items || logs) {
        const html = `  
        <div class="col-sm-4">
            <div class="card" data-id="${card.id}">
                <div class="card-header">${card.message}</div>
                <div class="card-body">
                    <p class="card-text">Message: ${card.errorText}</p>
                    <p class="card-text">Severity: ${card.severity}</p>
                    <button class="btn btn-outline-primary" data-id="${card.id}" data-operation="edit">Edit</button>
                    <button class="btn btn-danger" data-id="${card.id}" data-operation="delete">Delete</button>
                </div>
            </div>
        </div>`;
        $ph.append(html);
    }
}

function drawPagination(totalPages) {
    const $pagination = $("#pagination");
    $pagination.empty();

    for (let i = 1; i <= totalPages; i++) {
        const btn = `<li class="page-item ${i === currentPage ? 'active' : ''}">
                        <a class="page-link" href="#">${i}</a>
                     </li>`;
        $pagination.append(btn);
    }

    $(".page-link").click(function (e) {
        e.preventDefault();
        const selectedPage = parseInt($(this).text());
        fillLogs(selectedPage, pageSize);
    });
}
    $(document).ready(() => {
    $("#itemCount").change(function () {
        const newSize = parseInt($(this).val());
        fillLogs(1, newSize);
    });
        console.log(localStorage);
        let jwt = localStorage.getItem("JWT");
        console.log(jwt);
    fillLogs();
});                     