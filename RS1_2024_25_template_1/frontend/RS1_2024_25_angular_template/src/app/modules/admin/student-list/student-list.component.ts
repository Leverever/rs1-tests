import {Component, OnInit} from '@angular/core';
import {
  StudentGetAllEndpointService, StudentGetAllRequest,
  StudentGetAllResponse
} from '../../../endpoints/student-endpoints/student-get-all-endpoint.service';
import {MatSnackBar} from '@angular/material/snack-bar';
import {Router} from '@angular/router';
import {MyPagedRequest} from '../../../helper/my-paged-request';
import {MyPagedList} from '../../../helper/my-paged-list';
import {MatTableDataSource} from '@angular/material/table';
import {MyDialogConfirmComponent} from '../../shared/dialogs/my-dialog-confirm/my-dialog-confirm.component';
import {MatDialog} from '@angular/material/dialog';
import {PageEvent} from '@angular/material/paginator';
import {FormControl} from '@angular/forms';
import {debounceTime, filter} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {MyConfig} from '../../../my-config';

@Component({
  selector: 'app-student-list',
  standalone: false,

  templateUrl: './student-list.component.html',
  styleUrl: './student-list.component.css'
})


export class StudentListComponent implements OnInit {
  students : MyPagedList<StudentGetAllResponse> | null = null;
  pagedRequest: StudentGetAllRequest = {
    pageSize: 20,
    pageNumber: 1,
  }
  pageOptions = [20,50,75,100];
  dataSource = new MatTableDataSource<StudentGetAllResponse>();
  displayedColumns = ["ib", "firstName", "lastName", "actions"];

  searchQuery = new FormControl('');

  deletedCss = {
    'text-decoration': 'line-through',
    'color': 'gray'
  }

  showDeleted = true;

  constructor(public snackBar: MatSnackBar,
              private router: Router,
              private studentGetAll: StudentGetAllEndpointService,
              private dialog: MatDialog,
              private httpClient: HttpClient) {}

  ngOnInit(): void {
    this.loadStudents();

    this.searchQuery.valueChanges.pipe(
      debounceTime(300),
    ).subscribe(value => {
      this.pagedRequest.firstLast = value ?? undefined;
      this.loadStudents();
    })}

  editStudent(id:number) {
    this.router.navigate(['/admin/student', id]);
  }

  loadStudents() {
    this.studentGetAll.handleAsync(this.pagedRequest, true, 30000).subscribe({
      next: value => {
        this.students = value;
        this.dataSource.data = this.students.dataItems;
      }
    })
  }

  openMyConfirmDialog(id: number) {
    const dialogRef = this.dialog.open(MyDialogConfirmComponent, {
      width: '350px',
      data: {
        title: 'Potvrda brisanja',
        message: 'Da li ste sigurni da želite obrisati ovu stavku?',
        confirmButtonText: 'Da',
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

  private deleteStudent(id: number) {
    this.httpClient.delete<any>(`${MyConfig.api_address}/students/delete/${id}`).subscribe({
      next: value => {

        let deletedStudent = this.students?.dataItems.find(s=> s.id === value.id);
        if(deletedStudent)
        {
          deletedStudent.isDeleted = true;
        }
      },
      error: error => {console.log(error)}
    })
  }

  pageStudents(page: PageEvent) {
    this.pagedRequest.pageNumber = page.pageIndex+1;
    this.pagedRequest.pageSize = page.pageSize;
    this.loadStudents();
  }

  filterStudents() {
    this.showDeleted = !this.showDeleted;
    if(this.showDeleted)
    {
      this.dataSource.data = this.students!.dataItems;
    }
    else
    {
      this.dataSource.data = this.students!.dataItems.filter(val => !val.isDeleted);
    }
  }
}
