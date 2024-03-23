IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'visits')
BEGIN
	CREATE DATABASE [visits]
END
GO

USE [visits]
GO



IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='preregistrations' and xtype='U')
BEGIN
	CREATE TABLE preregistrations (
		guid VARBINARY(36) NOT NULL PRIMARY KEY,
		company_key NVARCHAR(20) NOT NULL,
		full_name NVARCHAR(100) NOT NULL,
		email NVARCHAR(100) NOT NULL,
		visit_date DATETIME NOT NULL,
		motive NVARCHAR(150),
		created_at DATETIME NOT NULL,
		confirmed_at DATETIME
	);
END
GO



IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='preregistrations_settings' and xtype='U')
BEGIN
	CREATE TABLE preregistrations_settings(
		id INTEGER NOT NULL IDENTITY PRIMARY KEY,
		link_expiration_hours SMALLINT NOT NULL,
		link_url_format NVARCHAR(200) NOT NULL,
		link_invalidate_after_confirm INTEGER NOT NULL,
		email_subject NVARCHAR(100) NOT NULL,
		email_body_format NVARCHAR(700) NOT NULL,
		email_body_labels_replace NVARCHAR(200) NOT NULL,
		smtp_host NVARCHAR(50) NOT NULL,
		smtp_port SMALLINT NOT NULL,
		smtp_user NVARCHAR(50) NOT NULL,
		smtp_password NVARCHAR(100) NOT NULL,
		lang_locale_default NVARCHAR(100) NOT NULL,
		lang_labels_version INTEGER NOT NULL
	);
END
GO



IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='locales' and xtype='U')
BEGIN
	CREATE TABLE locales(
		id NVARCHAR(5) PRIMARY KEY,
		name NVARCHAR(50) NOT NULL,
		selector_id NVARCHAR(5) NOT NULL,
		selector_name NVARCHAR(50) NOT NULL,
		date_time_format NVARCHAR(25) NOT NULL,
		date_time_format_front_end NVARCHAR(25) NOT NULL
	);
END
GO



IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='labels' and xtype='U')
BEGIN
	CREATE TABLE labels(
		id INTEGER NOT NULL IDENTITY PRIMARY KEY,
		locale_id NVARCHAR(5) NOT NULL,
		label NVARCHAR(40) NOT NULL,
		translation NVARCHAR(300) NOT NULL,
		FOREIGN KEY(locale_id) REFERENCES locales(id),
		UNIQUE(locale_id, label)
	);
END
GO



-- -- -- -- DEFAULT CONFIG -- -- -- --

INSERT INTO preregistrations_settings(link_expiration_hours, link_url_format, link_invalidate_after_confirm, email_subject, email_body_labels_replace, email_body_format, smtp_host, smtp_port, smtp_user, smtp_password, lang_locale_default, lang_labels_version) VALUES(0, '', 0, '', '','', '', 0, '', '', '', 0);

UPDATE preregistrations_settings SET link_expiration_hours = 			24;
UPDATE preregistrations_settings SET link_url_format = 					'Confirmation/Show/{0}';
UPDATE preregistrations_settings SET link_invalidate_after_confirm =	0;
UPDATE preregistrations_settings SET email_subject = 					'EMAIL_VCONFIRM_SUBJECT';
UPDATE preregistrations_settings SET email_body_labels_replace =		'LBL_COMP_KEY,LBL_FNAME,LBL_DATETIME,LBL_VMOTIVE,EMAIL_VCONFIRM_LINKTXT';
UPDATE preregistrations_settings SET email_body_format = 				'<p>LBL_COMP_KEY: <span style="font-weight: bold;">DATA_CompanyKey</span><br/>LBL_FNAME: <span style="font-weight: bold;">DATA_FullName</span><br/>LBL_DATETIME: <span style="font-weight: bold;">DATA_VisitDate</span><br/>LBL_VMOTIVE: <span style="font-weight: bold;">DATA_Motive</span><br/><a href="DATA_Link">EMAIL_VCONFIRM_LINKTXT</a><br/><br/><span style="font-weight: bold;"><a href="DATA_Link">DATA_Link</a></span></p>';
UPDATE preregistrations_settings SET lang_locale_default =				'es-MX';
UPDATE preregistrations_settings SET lang_labels_version =				1;
UPDATE preregistrations_settings SET smtp_host = 						'smtp.gmail.com';
UPDATE preregistrations_settings SET smtp_port = 						'587';
UPDATE preregistrations_settings SET smtp_user = 						'cortana@gmail.com';
UPDATE preregistrations_settings SET smtp_password = 					'spartan117';



-- -- -- -- LABELS -- -- -- --

INSERT INTO locales(id, name, selector_id, selector_name, date_time_format, date_time_format_front_end) VALUES('es-MX', 'Spanish (M&eacute;xico)', 'ES', 'Espa&ntilde;ol',	'dd/MM/yyyy hh:mm tt',	'DD-MM-YYYY hh:mm A');
INSERT INTO locales(id, name, selector_id, selector_name, date_time_format, date_time_format_front_end) VALUES('en-US', 'English (United States)', 'US', 'English',			'MM/dd/yyyy hh:mm tt',	'MM-DD-YYYY hh:mm A');

-- Labels for es-MX
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'EMAIL_VCONFIRM_SUBJECT',		'Solicitud de confirmaci&oacute;n de Visita');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'EMAIL_VCONFIRM_LINKTXT',		'Confirmar el preregistro');

INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'FOT_COPYRIGHT',				'Derechos Reservados 2024 &copy; SIASA | Sistemas Integrales de Automatizaci&oacute;n S.A. de C.V.');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'FOT_CONTACT',		 		'marketing@siasa.com | SIASA Matriz: (999) 930.2575 | SIASA CDMX: (55) 5264.2272 | SIASA Latam: (305) 479.2303');

INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'TTL_SCR_SUFFIX', 			' - Visitas SIASA');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'TTL_SCR_REQPRE',				'Solicitud de pre-registro de Visitante');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'TTL_SCR_CONFIRM',			'Confirmaci&oacute;n de visita');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'TTL_SCR_CONFIRMED',			'Visita confirmada');

INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'QST_ISALLDATAOK', 			'&iquest;Los datos proporcionados son correctos?');

INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_PROC_PLEASE_WAIT',		'procesando, espere un momento por favor');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_REQ_PREREG_OK',			'Se ha enviado correo a MAILTO para continuar con el pre-registro.');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_REQ_PREREG_ERR',			'La solicitud de pre-registro no se realiz&oacute; de forma exitosa, por favor reintente.');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_REQ_FIELD',				'Este campo es requerido');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_CAPTURECAPTCHA',			'Captura el c&oacute;digo de 4 d&iacute;gitos (Captcha) en la caja de texto');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_VCONFIRMED',				'La visita ya fu&eacute; confirmada, use el bot&oacute;n de abajo para ver el c&oacute;digo QR.');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_VARRIVE_EARLY',			'Favor de presentarse antes de la hora programada para su visita.');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_QRONHAND',				'Tenga a la mano el QR, se requiere para completar el registro cuando se presente a su visita.');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_INVALID_LINK',			'Solicitud de pre-registro inv&aacute;lido.');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_VDATE_EXPIRED',			'La fecha de la visita ya ha pasado.');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_VLINK_EXPIRED',			'El enlace de confirmaci&oacute;n ha dejado de estar vigente.');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_EMAIL_INVALID',			'El correo electr&oacute;nico no es v&aacute;lido');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_COMPKEY_LENGTH',			'Este campo debe tener entre 1 y 20 caracteres');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_FNAME_LENGTH',			'Este campo debe tener entre 3 y 100 caracteres');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_EMAIL_LENGTH',			'Este campo debe tener entre 5 y 100 caracteres');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_MOTIV_LENGTH',			'Este campo debe tener como m&aacute;ximo 150 caracteres');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_CAPTCHA_WRONG',			'El captcha no es correcto');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_DTGREATER_TODAY',		'La fecha/hora debe ser mayor a la actual');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_REQ_SENT',				'Solicitud enviada');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_ONEVIEW_QR',				'&iexcl;El QR ya no estar&aacute; disponible, capture, descargue o impr&iacute;malo antes de cerrar esta ventana!');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'MSG_MAILBOX_UNAVAIL',		'La direcci&oacute;n de correo proporcionada no existe o no esta disponible.');

INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_VISITS',					'visitas');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_COMP_KEY',				'clave de empresa');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_FNAME',					'nombre completo');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_EMAIL',					'correo electr&oacute;nico');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_DATETIME',				'fecha y hora');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_VMOTIVE',				'motivo de visita');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_SEND_EMAIL_PREREQ',		'enviar correo para iniciar el pre-registro');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_CONFIRMATION', 			'confirmaci&oacute;n');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_YES',			 		'si');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_NO',						'no');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_OK',						'aceptar');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_ERROR',					'error');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_CAPTCHA',				'Captcha');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_SHOW_QR',				'Mostrar QR');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_CONFIRM_VISIT',			'Confirmar visita');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_PRINT',					'imprimir');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_DOWNLOAD',				'descargar');
INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_CLOSE',					'cerrar');
-- INSERT INTO labels(locale_id, label, translation) VALUES('es-MX', 'LBL_',	'');


-- Labels for en-US
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'EMAIL_VCONFIRM_SUBJECT',		'Visit confirmation request');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'EMAIL_VCONFIRM_LINKTXT',		'Confirm pre-registration');

INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'FOT_COPYRIGHT',				'All Rights Reserved 2024 &copy; SIASA | Sistemas Integrales de Automatizaci&oacute;n S.A. de C.V.');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'FOT_CONTACT', 				'marketing@siasa.com | SIASA Headquarters: (999) 930.2575 | SIASA CDMX: (55) 5264.2272 | SIASA Latin America: (305) 479.2303');

INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'TTL_SCR_SUFFIX', 			' - Visits SIASA');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'TTL_SCR_REQPRE',				'Visitor pre-registration request');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'TTL_SCR_CONFIRM',			'Visit confirmation');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'TTL_SCR_CONFIRMED',			'Visit confirmed');

INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'QST_ISALLDATAOK', 			'Is the data provided correct?');

INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_PROC_PLEASE_WAIT',		'processing, please wait');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_REQ_PREREG_OK',			'An email was sent to the address MAILTO in order to continue with the pre-registration.');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_REQ_PREREG_ERR',			'The pre-registration request was not successful, please retry.');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_REQ_FIELD',				'This field is required');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_CAPTURECAPTCHA',			'Capture the 4-digit code (Captcha) in the text box');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_VCONFIRMED',				'The visit has already been confirmed, use the button below to view the QR code.');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_VARRIVE_EARLY',			'Please arrive before your scheduled visit time.');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_QRONHAND',				'Please have the QR ready, it is required to complete registration when you show up for your visit.');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_INVALID_LINK',			'Invalid pre-registration request.');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_VDATE_EXPIRED',			'The visit date has already passed.');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_VLINK_EXPIRED',			'The confirmation link has expired.');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_EMAIL_INVALID',			'The email is invalid');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_COMPKEY_LENGTH',			'This field must be between 1 and 20 characters');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_FNAME_LENGTH',			'This field must be between 3 and 100 characters');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_EMAIL_LENGTH',			'This field must be between 5 and 100 characters');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_MOTIV_LENGTH',			'This field must have a maximum of 150 characters');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_CAPTCHA_WRONG',			'The captcha code is wrong');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_DTGREATER_TODAY',		'The date/time must be greater than the current one');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_REQ_SENT',				'The request was sent');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_ONEVIEW_QR',				'The QR will no longer be available, please capture, download or print it before closing this window!');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'MSG_MAILBOX_UNAVAIL',		'The email address provided does not exist or is not available.');

INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_VISITS',					'visits');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_COMP_KEY',				'company key');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_FNAME',					'full name');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_EMAIL',					'email');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_DATETIME',				'date and time');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_VMOTIVE',				'visit motive');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_SEND_EMAIL_PREREQ',		'send email to start pre-registration');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_CONFIRMATION', 			'confirm');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_YES',					'yes');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_NO',						'no');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_OK',						'ok');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_ERROR',					'error');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_CAPTCHA',				'Captcha');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_SHOW_QR',				'Show QR');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_CONFIRM_VISIT',			'Confirm visit');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_PRINT',					'print');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_DOWNLOAD',				'download');
INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_CLOSE',					'close');
-- INSERT INTO labels(locale_id, label, translation) VALUES('en-US', 'LBL_',	'');
GO