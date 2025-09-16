document.addEventListener('DOMContentLoaded', function () {
    // 自動為每個 sortable th 加上 data-sort-col
    document.querySelectorAll('table').forEach(table => {
        const ths = table.querySelectorAll('thead th');
        ths.forEach((th, idx) => {
            if (th.classList.contains('sortable')) {
                th.setAttribute('data-sort-col', idx);
            }
        });
    });

    // 排序功能
    document.querySelectorAll('th.sortable').forEach(th => {
        th.addEventListener('click', function () {
            const colIdx = parseInt(th.getAttribute('data-sort-col'));
            const table = th.closest('table');
            const tbody = table.querySelector('tbody');
            const rows = Array.from(tbody.querySelectorAll('tr'));
            const indicator = th.querySelector('.sort-indicator');

            let asc = !th.classList.contains('sorted-asc');
            // 清除所有排序狀態
            table.querySelectorAll('th.sortable').forEach(t => {
                t.classList.remove('sorted-asc', 'sorted-desc');
                const ind = t.querySelector('.sort-indicator');
                if (ind) ind.textContent = '';
            });

            // 判斷型態：日期、數字、字串
            function detectType(val) {
                // 日期型態
                if (!isNaN(Date.parse(val)) && val.match(/\d{4}/)) return 'date';
                // 數字型態
                if (!isNaN(val) && val.trim() !== '') return 'number';
                return 'string';
            }

            // 取得第一個非空值判斷型態
            let type = 'string';
            for (let r of rows) {
                let v = r.children[colIdx].textContent.trim();
                if (v) {
                    type = detectType(v);
                    break;
                }
            }

            // 排序
            rows.sort((a, b) => {
                let aText = a.children[colIdx].textContent.trim();
                let bText = b.children[colIdx].textContent.trim();

                if (type === 'date') {
                    aText = Date.parse(aText) || 0;
                    bText = Date.parse(bText) || 0;
                } else if (type === 'number') {
                    aText = parseFloat(aText) || 0;
                    bText = parseFloat(bText) || 0;
                }
                if (asc) {
                    return aText > bText ? 1 : aText < bText ? -1 : 0;
                } else {
                    return aText < bText ? 1 : aText > bText ? -1 : 0;
                }
            });

            // 重新插入排序後的 row
            rows.forEach(row => tbody.appendChild(row));

            // 標記排序狀態
            th.classList.add(asc ? 'sorted-asc' : 'sorted-desc');
            if (indicator) indicator.textContent = asc ? '▲' : '▼';
        });
    });
});