import { FormControl } from '@angular/forms';
import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-create-temperature',
  templateUrl: './create-temperature.component.html',
  styleUrls: ['./create-temperature.component.css']
})
export class CreateTemperatureComponent {

  temperature = new FormControl('');

  public graphCollection: GraphDataCollection;


  constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string) { }



  SaveNewReading() {
    this.temperature.setValue('');

    this.http.get<GraphDataCollection>(this.baseUrl + 'api/TemperatureGraph/CreateBodyTemperature').subscribe(result => {
      this.graphCollection = result;
    }, error => console.error(error));
  }


  AddTemperature(temperatureDto: TemperatureDto) {

    this.http.post(this.baseUrl + 'api/TemperatureGraph/CreateBodyTemperature', {
      temperatureDto
    })
      .subscribe(
        res => {
          console.log(res);
        },
        err => {
          console.log("Error occured when saving new temperature reading");
        }
      );
  }
}


interface TemperatureDto {
  temperatureValue: number;
  readingDate: string;
  unitOfMessure: string;
}

interface GraphData {
  data: Array<number>;
  label: string;
}

interface GraphDataCollection {
  weatherList: GraphData[];
  chartLabels: string[];
}

