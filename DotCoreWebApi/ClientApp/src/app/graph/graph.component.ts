import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-graph',
  templateUrl: './graph.component.html',
  styleUrls: ['./graph.component.css']
})
export class GraphComponent {

  public weatherForecast: WeatherForecast;
  public chartLegend: boolean = true;
  public chartType: string = 'line';  


  public chartOptions: any = {
    responsive: true,
    legend: {
      position: 'bottom'
    }
  };


  //constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
  //  http.get(baseUrl + 'api/SampleData/GetWeatherForecast').subscribe(result => {
  //    this.weatherForecast = result.json() as WeatherForecast;
  //  }, error => console.error(error));
  //}


  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<WeatherForecast>(baseUrl + 'api/SampleData/GetWeatherForecast').subscribe(result => {
      this.weatherForecast = result;
    }, error => console.error(error));
  }

  
}


interface Weather {
  data: Array<number>;
  label: string;
} 

interface WeatherForecast {
  weatherList: Weather[];
  chartLabels: string[];
}  
