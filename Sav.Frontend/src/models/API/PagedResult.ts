export interface PagedResult<T> {
    page: number;
    count: number;
    totalCount: number;
    items: T[];
}