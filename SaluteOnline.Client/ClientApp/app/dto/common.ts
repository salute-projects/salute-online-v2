export class Page<TEntity> {
    currentPage: number;
    pageSize: number;
    total: number;
    totalPages: number;
    totalItems: number;
    items: Array<TEntity>;
}

export class Country {
    name: string;
    code: string;
}

export class BaseFilter {
    pageSize: number | null;
    page: number;
    asc: boolean;
    orderBy: string;
}

export class EntityFilter extends BaseFilter {
    constructor(id: number) {
        super();
        this.entityId = id;
        this.asc = false;
        this.searchBy = '';
        this.page = 1;
        this.pageSize = 25;
        this.orderBy = "";
    }

    entityId: number | null;
    searchBy: string;
}