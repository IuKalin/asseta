import 'package:equatable/equatable.dart';

class ActionContactEntity extends Equatable {
  final String id;
  final String actionCardId;
  final String contactName;
  final String relationshipOrRole;
  final String? phoneNumber;
  final String? email;
  final String? contactNotes;

  const ActionContactEntity({
    required this.id,
    required this.actionCardId,
    required this.contactName,
    required this.relationshipOrRole,
    this.phoneNumber,
    this.email,
    this.contactNotes,
  });

  factory ActionContactEntity.fromJson(Map<String, dynamic> json) {
    return ActionContactEntity(
      id: json['id'] as String,
      actionCardId: json['actionCardId'] as String,
      contactName: json['contactName'] as String,
      relationshipOrRole: json['relationshipOrRole'] as String,
      phoneNumber: json['phoneNumber'] as String?,
      email: json['email'] as String?,
      contactNotes: json['contactNotes'] as String?,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'actionCardId': actionCardId,
      'contactName': contactName,
      'relationshipOrRole': relationshipOrRole,
      'phoneNumber': phoneNumber,
      'email': email,
      'contactNotes': contactNotes,
    };
  }

  @override
  List<Object?> get props => [
        id,
        actionCardId,
        contactName,
        relationshipOrRole,
        phoneNumber,
        email,
        contactNotes,
      ];
}
