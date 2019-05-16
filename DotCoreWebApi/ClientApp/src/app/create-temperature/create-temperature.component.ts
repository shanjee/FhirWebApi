import { FormControl } from '@angular/forms';
import { FormBuilder, Validators } from '@angular/forms';
import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

import { TemperatureModel } from '../model/temperature-model';

@Component({
  selector: 'app-create-temperature',
  templateUrl: './create-temperature.component.html',
  styleUrls: ['./create-temperature.component.css']
})
export class CreateTemperatureComponent implements OnInit {

  temperature = new FormControl('');
  temperatureForm: any;

  public graphCollection: GraphDataCollection;
  private savedRespone: SavedRespone = {
    resourceType: '', comments: '', id :''
  };

  temperatureModel: Object = {
    readingValue: '',
    unit: '',
    dateOfReading: ''
  };

  ngOnInit() {
    this.temperatureForm = this.formbulider.group({
      Temperature: ['', [Validators.required]],
      UnitOfMessure: ['', [Validators.required]]
    });

  }

  onFormSubmit() {

    const temperatureData = this.temperatureForm.value;
    this.createTemperature(temperatureData);
    this.temperatureForm.reset();
  }


  constructor(private formbulider: FormBuilder, public http: HttpClient, @Inject('BASE_URL') public baseUrl: string) { }


  aveNewReading() {

    this.http.get<GraphDataCollection>(this.baseUrl + 'api/TemperatureGraph/CreateBodyTemperature').subscribe(result => {
      this.graphCollection = result;
    }, error => console.error(error));
  }


  AddTemperature(temperatureValue, dateOfReading) {

    this.http.post(this.baseUrl + 'api/TemperatureGraph/CreateBodyTemperature', {
      temperatureValue, dateOfReading
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

  createTemperature(temperature: TemperatureModel) {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

    //this.http.post(this.baseUrl + 'api/TemperatureGraph/CreateBodyTemperature',
    // JSON.stringify(temperature)).subscribe();

    this.http.post<SavedRespone>(this.baseUrl + 'api/TemperatureGraph/CreateBodyTemperature', JSON.stringify(temperature), httpOptions)
      .subscribe(res => {
        this.savedRespone = res;
      });
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

interface SavedRespone {
  resourceType: string;
  id: string;
  comments: string;
}

