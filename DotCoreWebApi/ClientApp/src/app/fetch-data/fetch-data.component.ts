import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public bodyTempList: BodyTemperatureStats[];
  public BloodPressureFhirStatsList: BloodPressureFhirStats[];

  //  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
  //    http.get<BodyTemperatureStats[]>(baseUrl + 'api/SampleData/GetBodyTemperature').subscribe(result => {
  //      this.bodyTempList = result;
  //    }, error => console.error(error));
  //  }
  //}

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<BloodPressureFhirStats[]>(baseUrl + 'api/SampleData/GetBloodPressureFromAql').subscribe(result => {
      this.BloodPressureFhirStatsList = result;
    }, error => console.error(error));
  }
}

interface BodyTemperatureStats {
  dateFormatted: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

interface BloodPressureFhirStats {
  dateTaken: string;
  diastolic: number;
  documentId: string;
  systolic: number;
  unitOfMessure: string;

}

