
document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('tableFilterModal').addEventListener('show.bs.modal', function () {
        const tableId = 'listTable';
        const theadCells = document.querySelectorAll(`#${tableId} thead th`);
        const modalBody = document.getElementById('filterModalBody');
        modalBody.innerHTML = '';
        theadCells.forEach((th, idx) => {
            if (th.textContent.trim() !== '') {
                modalBody.innerHTML += `
                           <div class="mb-2">
                             <label>${th.textContent}</label>
                             <input type="text" class="form-control" data-filter-col="${idx}" />
                           </div>
                         `;
            }
        });
    });

    document.getElementById('applyFilterBtn').addEventListener('click', function () {
        const tableId = 'listTable';
        const filters = {};
        document.querySelectorAll('#filterModalBody input').forEach(input => {
            const val = input.value.trim();
            if (val) filters[input.dataset.filterCol] = val;
        });
        document.querySelectorAll(`#${tableId} tbody tr`).forEach(row => {
            let show = true;
            Object.entries(filters).forEach(([colIdx, val]) => {
                const cell = row.children[colIdx];
                if (!cell.textContent.includes(val)) show = false;
            });
            row.style.display = show ? '' : 'none';
        });
        bootstrap.Modal.getInstance(document.getElementById('tableFilterModal')).hide();
    });
});

document.getElementById('clearFilterBtn').addEventListener('click', function () {
    // 清除角色篩選
    selectedRoles = [];
    document.querySelectorAll('.filter-btn[data-role]').forEach(btn => btn.classList.remove('active'));

    // 清除啟用篩選
    activeChecked = false;
    document.querySelectorAll('.filter-btn[data-active]').forEach(btn => btn.classList.remove('active'));

    // 清除進階篩選 modal 欄位
    document.querySelectorAll('#filterModalBody input').forEach(input => input.value = '');

    // 顯示所有資料列
    document.querySelectorAll('#listTable tbody tr').forEach(row => row.style.display = '');

});