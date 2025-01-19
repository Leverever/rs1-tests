import {Component, OnInit} from '@angular/core';
import {Router} from "@angular/router";
import {
  StudentGetAllEndpointService, StudentGetAllRequest,
  StudentGetAllResponse
} from "../../../endpoints/student-endpoints/student-get-all-endpoint.service";
import {MyPagedList} from "../../../helper/my-paged-list";
import {MatTableDataSource} from "@angular/material/table";
import {MyDialogConfirmComponent} from "../../shared/dialogs/my-dialog-confirm/my-dialog-confirm.component";
import {MatDialog} from "@angular/material/dialog";
import {FormControl} from "@angular/forms";
import {debounceTime} from "rxjs";
import {PageEvent} from "@angular/material/paginator";
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {MyConfig} from "../../../my-config";
import {MatSnackBar} from "@angular/material/snack-bar";

@Component({
  selector: 'app-student-list',
  standalone: false,

  templateUrl: './student-list.component.html',
  styleUrl: './student-list.component.css'
})
export class StudentListComponent implements OnInit{
  students : MyPagedList<StudentGetAllResponse> | null = null;
  dataSource = new MatTableDataSource<StudentGetAllResponse>()
  displayedColumns = ["ib", "firstLast", "municipality", "actions"]

  pagedRequest : StudentGetAllRequest = {
    pageNumber: 1,
    pageSize: 20,
  }

  pageOptions = [20,35,50,100]

  firstLast = new FormControl<string | null>(null);

  obrisan = {
    'text-decoration': 'line-through',
    'color':'gray'
  }

  showDeleted = true;


  constructor(private router: Router,
              private studentGetAll: StudentGetAllEndpointService,
              private dialog: MatDialog,
              private http: HttpClient,
              private snackBar: MatSnackBar) {
  }

  ngOnInit(): void {
    this.loadStudents();

    this.firstLast.valueChanges.pipe(debounceTime(300)).subscribe({
      next: (data) => {
        this.pagedRequest.firstLast = data ?? undefined;
        this.loadStudents();
      }
    })
  }

  loadStudents(){
    this.studentGetAll.handleAsync(this.pagedRequest, true, 30000).subscribe({
      next: (data) => {
        this.students = data;
        this.dataSource.data = this.students.dataItems
      }
    })
  }

  editStudent(id:number) {
    this.router.navigate(['/admin/student/',id]);
  }

  openMyConfirmDialog(id: number) {
    const dialogRef = this.dialog.open(MyDialogConfirmComponent, {
      width: '350px',
      data: {
        title: 'Potvrda brisanja',
        message: 'Da li ste sigurni da želite obrisati ovog studenta?',
        confirmButtonText: "Da"
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log('Korisnik je potvrdio brisanje');
        // Pozovite servis ili izvršite logiku za brisanje
        this.deleteStudent(id);
      } else {
        console.log('Korisnik je otkazao brisanje');
      }
    });
  }

  deleteStudent(id:number) {
    this.http.delete<StudentGetAllResponse>(`${MyConfig.api_address}/students/delete/${id}`).subscribe({
      next: (student) => {
        this.snackBar.open(`Soft deleted ${student.studentNumber} successfully.`, "", {duration: 2000});
        let s = this.students!.dataItems.find(val => val.id === student.id);
        if(s)
        {
          s.isDeleted = true;
        }
      },
      error: (err :HttpErrorResponse) => this.snackBar.open(err.message, "", {duration: 2000})
    })
  }

  pageEvent(page: PageEvent) {
    this.pagedRequest.pageSize = page.pageSize
    this.pagedRequest.pageNumber = page.pageIndex+1;
    this.loadStudents();
  }

  toggleDeleted() {
    this.showDeleted = !this.showDeleted;
    if(this.showDeleted)
    {
      this.dataSource.data = this.students!.dataItems;
    }
    else {
      this.dataSource.data = this.students!.dataItems.filter(s => !s.isDeleted)
    }
  }
}
