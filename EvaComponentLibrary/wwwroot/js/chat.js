function getCurrentTheme() {
	
	let theme = window.matchMedia('(prefers-color-scheme:dark)').matches ? 'dark' : 'light';
	return theme;
	localStorage.getItem('mode-theme') ? theme = localStorage.getItem('mode-theme') : null
	console.log(theme);
}


function loadTheme(theme) {
	const root = document.querySelector(':root');
	root.setAttribute('color-scheme', `${theme}`);
}

function changeTheme(color) {
	/*localStorage.setItem('mode-theme', `${color}`);*/
	loadTheme(color);
}


window.addEventListener('DOMContentLoaded', () => {
	loadTheme(getCurrentTheme());
})

function AutoScrollToElem() {
	const elem = document.getElementById('scroll-to-elem');
	if (elem) {
		elem.scrollIntoView({ behavior: 'smooth' });
	} else {
		console.warn("Element with id 'scroll-to-elem' not found.");
	}
}






//----------------------------------------------------------------- PDF Viewer ---------------------------------------------------------------------------- 

function loadPdfIntoIframe(byteArray, initPage) {
	const pdfData = byteArray;
	var pdfjsframe = document.getElementById("pdfjsframe");
	const interval = setInterval(() => {
		const viewerWindow = pdfjsframe.contentWindow;
		const app = viewerWindow?.PDFViewerApplication;
		if (app && app.initialized) {
			clearInterval(interval);
			localStorage.removeItem('pdfjs.history');
			app.eventBus.on("pagesloaded", () => {
				app.pdfViewer.currentPageNumber = initPage;
			});
			app.open({ data: pdfData });

		}
	}, 100);
}

//----------------------------------------------------------------- OpenLink ---------------------------------------------------------------------------- 

window.openLinkInNewTab = (url) => {
	window.open(url, '_blank');
};

//----------------------------------------------------------------- BreadCrumbs -------------------------------------------------------------------------

function breadCrumbsEllipsis() {
    const path = document.getElementById('path');
    const ul = document.getElementById('segment');

    let isUpdating = false; 

    function checkOverflow() {
        if (isUpdating) return; 

        isUpdating = true;

        path.classList.remove('collapsed');
        requestAnimationFrame(() => {
            const pathWidth = path.getBoundingClientRect().width;
            const ulWidth = ul.getBoundingClientRect().width;
            const needsCollapse = ulWidth > (pathWidth) - 5 ;

            if (needsCollapse) {
                path.classList.add('collapsed');
            }

            
            isUpdating = false;
        });
    }

    const resizeObserver = new ResizeObserver(checkOverflow);
    resizeObserver.observe(path);

    const mutationObserver = new MutationObserver((mutations) => {

        const hasContentChanges = mutations.some(mutation =>
            mutation.type === 'childList'
        );

        if (hasContentChanges) {
            checkOverflow();
        }
    });

    mutationObserver.observe(ul, {
        childList: true,       
        subtree: true,          
        attributes: false     
    });


    window.addEventListener('resize', checkOverflow);

    checkOverflow();

}
function clean () {
    resizeObserver.disconnect();
    mutationObserver.disconnect();
    window.removeEventListener('resize', checkOverflow);
};
