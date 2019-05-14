import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-graph',
  templateUrl: './graph.component.html',
  styleUrls: ['./graph.component.css']
})
export class GraphComponent {

  public graphCollection: GraphDataCollection;
  public chartLegend: boolean = true;
  public chartType: string = 'line';  


  public chartOptions: any = {
    responsive: true,
    legend: {
      position: 'bottom'
    }
  };

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<GraphDataCollection>(baseUrl + 'api/SampleData/GetWeatherForecast').subscribe(result => {
      this.graphCollection = result;
    }, error => console.error(error));
  }
}


interface GraphData {
  data: Array<number>;
  label: string;
} 

interface GraphDataCollection {
  weatherList: GraphData[];
  chartLabels: string[];
}  
