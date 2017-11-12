export var menuList = [
    {
        path: 'fetch-data',
        data: {
            menu: {
                title: 'Home',
                icon: 'home'
            }
        }
    },
    {
        path: 'so-clubs-list',
        data: {
            menu: {
                title: 'Clubs',
                icon: 'people'
            }
        }
    },
    {
        path: 'materials',
        data: {
            menu: {
                title: 'Матеріали',
                icon: 'list',
                selected: false,
                expanded: false,
                order: 100,
            }
        },
        children: [
            {
                path: 'codex',
                data: {
                    menu: {
                        title: 'Кодекс клубу',
                        faicon: 'fa-book'
                    }
                }
            },
            {
                path: 'blogs',
                data: {
                    menu: {
                        title: 'Блоги',
                        faicon: 'fa-file-text-o'
                    }
                }
            },
            {
                path: 'articles',
                data: {
                    menu: {
                        title: 'Статті',
                        faicon: 'fa-file-text-o'
                    }
                }
            }
        ]
    }
];
//# sourceMappingURL=menu-list.js.map