import { SubFilter } from "../../../services/models/filter-helper/sub-filter";

export interface FilterQueryModel {
    'Filter.Logic': string;
    'Filter.Filters': Array<SubFilter>;
}