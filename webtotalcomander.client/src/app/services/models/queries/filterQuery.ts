import { SubFilter } from "../filter-helper/sub-filter";

export class FilterQuery {
    public 'Filter.Logic': string = '';
    public 'Filter.Filters': Array<SubFilter> = [];
}
